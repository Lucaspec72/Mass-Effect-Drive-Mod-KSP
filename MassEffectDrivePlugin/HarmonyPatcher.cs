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
