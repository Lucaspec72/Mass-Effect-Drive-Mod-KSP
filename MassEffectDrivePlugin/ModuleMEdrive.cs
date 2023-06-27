using System;
using UnityEngine;
using MassEffectDrivePlugin;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace MassEffectDrivePlugin
{
    public class ModuleMEdrive : PartModule
    {
        //Define local reference to VesselModule
        VesselModuleMEdrive VesselDrive;

        //Display for drive status, not critical to functionality.
        [KSPField(guiName = "Mass Effect Drive Status", guiActive = true, isPersistant = false)]
        public string MEdriveState = "Inactive";

        [KSPField(guiActive = true, guiActiveEditor = false, guiName = "Mass Reduction Percentage", isPersistant = false), UI_FloatRange(minValue = -98f, maxValue = 98f, stepIncrement = 0.01f)]
        public float GravWarpHandle = 0f;
        //stores the previous value of GravWarpHandle to detect change.
        public float GravWarpHandlePrevious = 0f;

        //figure out how to add unit to display.
        [KSPField(guiName = "Real Mass", guiActive = true, isPersistant = false)]
        public float VesselMass = 0;

        [KSPField(guiName = "Warped Mass", guiActive = true, isPersistant = false)]
        public float WarpedMass = 0;

        [KSPField(guiName = "Current Energy Requirement", guiActive = true, isPersistant = false)]
        public double DriveConsumption = 0;

        //ADD EFFICIENCY MULTIPLIER (percentage of optimal eezo)

        [KSPAction("ToggleMEdrive", KSPActionGroup.None, guiName = "Toggle Mass Effect Drive")]
        private void ToggleMEdrive(KSPActionParam param)
        {
            VesselDrive.ToggleDrive();
        }

        [KSPEvent(name = "ActivateMEdrive", guiName = "Activate Mass Effect Drive", active = true, guiActive = true)]
        public void ActivateMEdrive()
        {
            if (MEdriveState == "Inactive")
            {
                VesselDrive.ToggleDrive();
            }
        }
        
        [KSPEvent(name = "DeactivateMEdrive", guiName = "Deactivate Mass Effect Drive", active = true, guiActive = false)]
        public void DeactivateMEdrive()
        {
            if (MEdriveState == "Active")
            {
                VesselDrive.ToggleDrive();
            }
        }


        //Refreshes the VesselDrive value. (also called by VesselModule each time it updates it's DriveList.)
        public void UpdateVesselDrive()
        {
            VesselDrive = this.vessel.FindVesselModuleImplementing<VesselModuleMEdrive>();
        }

        public void SendUpdateWarpPercentage()
        {
            VesselDrive.GravWarpPercentage = GravWarpHandle;
            VesselDrive.UpdateHandleGravWarpPercentage();
            GravWarpHandlePrevious = GravWarpHandle;
        }

        public override void OnStart(PartModule.StartState state)
        {
            base.OnStart(state);
            {
                if (state != StartState.Editor && state != StartState.None)
                {
                    this.enabled = true;
                    this.part.force_activate();
                } else
                {
                    this.enabled = false;
                }
            }
        }
        


        public override void OnFixedUpdate()
        {
            if (GravWarpHandle != GravWarpHandlePrevious)
            {
                SendUpdateWarpPercentage();
            }
            //Putting this here might be a bad idea, idk.
            //actualMass = MassHandler.GetVesselWetMass(this.vessel); //only get part mass to match current baseMass variable.
            //VesselModuleMEdrive test = this.vessel.FindVesselModuleImplementing<VesselModuleMEdrive>();
            //baseMassStr = baseMass.ToString("R");
            //actualMassStr = actualMass.ToString("R");
            //percentageMass = (100-(actualMass/baseMass)*100).ToString("R")+"%";
        }

    }
}