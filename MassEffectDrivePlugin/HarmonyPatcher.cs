using HarmonyLib;
using System;
using UnityEngine;
using MassEffectDrivePlugin;

namespace MassEffectDrivePlugin
{
    [KSPAddon(KSPAddon.Startup.Instantly, true)]
    public class MEdrivePluginHarmonyPatcher : MonoBehaviour
    {
        void Start()
        {
            var harmony = new Harmony("com.Lucaspec72.MEDrivePlugin.CustomPartMassModifier");
            harmony.PatchAll();
        }
        [HarmonyPatch(typeof(Part), nameof(Part.UpdateMass))]
        public class UpdateMass_Patch
        {
            static bool Prefix(Part __instance)
            {
                int count = __instance.modules.Count;
                __instance.moduleMass = 0.0f;
                __instance.inventoryMass = 0.0f;
                if (__instance.needPrefabMass)
                {
                    __instance.prefabMass = __instance.mass;
                    if (__instance.partInfo != null && (UnityEngine.Object)__instance.partInfo.partPrefab != (UnityEngine.Object)null)
                        __instance.prefabMass = __instance.partInfo.partPrefab.mass;
                    __instance.needPrefabMass = false;
                }
                for (int index = 0; index < count; ++index)
                {
                    if (__instance.modules[index] is IPartMassModifier module)
                    {
                        float moduleMass = module.GetModuleMass(__instance.prefabMass, ModifierStagingSituation.CURRENT);
                        __instance.moduleMass += moduleMass;
                        if (__instance.modules[index] is ModuleInventoryPart)
                            __instance.inventoryMass += moduleMass;
                    }
                }
                //CODE INJECTION
                float customMassModifier = 0;

                if (HighLogic.LoadedSceneIsFlight)
                {
                    VesselModuleMEdrive MEdrive = __instance.vessel.FindVesselModuleImplementing<VesselModuleMEdrive>();
                    if (MEdrive)
                    {
                        customMassModifier = -((MEdrive.GetMassPercentage() / 100) * ((__instance.prefabMass + __instance.moduleMass) + __instance.GetResourceMass()));
                        MEdrive.PartMass[__instance] = __instance.prefabMass + __instance.moduleMass + __instance.GetResourceMass();
                    }
                }   
                __instance.mass = __instance.prefabMass + __instance.moduleMass + customMassModifier;
                if (customMassModifier != 0)
                    return false;
                //CODE INJECTION
                if ((double)__instance.mass >= (double)__instance.partInfo.MinimumMass)
                    return false;
                __instance.mass = __instance.partInfo.MinimumMass;
                return false;
            }
        }
        [HarmonyPatch(typeof(Part), nameof(Part.ModulesOnFixedUpdate))]
        public class ModulesOnFixedUpdate_Patch
        {
			static void Postfix(Part __instance)
			{
				float customMassModifier = 0;

				if (HighLogic.LoadedSceneIsFlight) //might edit this check at some point to apply this when the vessel exists rather than when in flight mode. (DUCT TAPE FIX)
				{
					VesselModuleMEdrive MEdrive = __instance.vessel.FindVesselModuleImplementing<VesselModuleMEdrive>();
					if (MEdrive)
					{
						customMassModifier = -((MEdrive.GetMassPercentage() / 100) * (__instance.mass + __instance.GetResourceMass()));
						MEdrive.PartMass[__instance] = __instance.mass + __instance.GetResourceMass();
					}
				}

				__instance.mass += customMassModifier;
			}

		}
    }
}
