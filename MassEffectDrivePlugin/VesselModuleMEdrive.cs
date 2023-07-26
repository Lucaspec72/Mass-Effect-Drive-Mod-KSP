using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MassEffectDrivePlugin
{
    public class VesselModuleMEdrive : VesselModule
    {
        //---NOTES---
        //planned features :
        //static energy, generated at same rate as EC consumption, stored in condensators, released in low orbit.

        //ideas :
        //Mass relay : Allows teleportation to another mass relay. (not priority, since ESLD Jump Beacons with mass effect drives on ship gives pretty much same result.)
        //way of releasing static energy in deep space.






        //list of all of a vessel's drives
        public List<ModuleMEdrive> DriveList = new List<ModuleMEdrive>();
        //used to determine real mass of vessel
        public Dictionary<Part, float> PartMass = new Dictionary<Part, float>();
        //hashes for ressource requests
        int EChash;
        int Fluxhash;
        //hard value equal to DriveList.Count() to get number of drives without needing drivelist, for unloaded calcs.
        public int DriveCount;
        //Mass warping percentage
        public float GravWarpPercentage;
        //real mass of vessel
        public float VesselMass;
        public float VesselMassPrevious;
        public float WarpedMassPrevious;
        public double DriveConsumptionPrevious;
        public double FluxVentingMultPrevious;
        //Boolean flag for drive state
        public bool MEdriveActivated = false;
 

        //getter for the harmony patch.
        public float GetMassPercentage()
        {
            if (MEdriveActivated)
            {
                return GravWarpPercentage;
            }
            else
            {
                return 0f;
            }
        }

        public double CalcEnergy()
        {
            double eezoMass = 0;
            //might want to upgrade this at some point to actually get the Eezo mass, it currently gets the mass of all ressources in the drive parts, so any MM patch that adds ressources to all parts would fuck that up.
            foreach (ModuleMEdrive Drive in DriveList)
            {
                eezoMass += Drive.part.GetResourceMass();
            }
            double energyCost = (((GravWarpPercentage/100) * VesselMass) / (96.5 * eezoMass) + 0.001) * 3;

            return energyCost;
        }

        public void ToggleDrive()
        {
            if (MEdriveActivated)
            {
                DisableDrive();
            } else
            {
                EnableDrive();
            }
        }

        public void DisableDrive()
        {
            MEdriveActivated = false;
            foreach (ModuleMEdrive Drive in DriveList)
            {
                Drive.MEdriveState = "Inactive";
                Drive.Events["DeactivateMEdrive"].guiActive = false;
                Drive.Events["ActivateMEdrive"].guiActive = true;
                Drive.WarpedMass = 0;
            }
        }
        public void EnableDrive()
        {
            //placeholder code, will add variable MEdriveActivable variable and code to handle it to check whether activation requirements are met.
            MEdriveActivated = true;
            foreach (ModuleMEdrive Drive in DriveList)
            {
                
                Drive.MEdriveState = "Active";
                Drive.Events["DeactivateMEdrive"].guiActive = true;
                Drive.Events["ActivateMEdrive"].guiActive = false;
            }
        }

        public void UpdateHandleGravWarpPercentage()
        {
            foreach (ModuleMEdrive Drive in DriveList)
            {
                Drive.GravWarpHandlePrevious = GravWarpPercentage;
                Drive.GravWarpHandle = GravWarpPercentage;
            }
        }


        //clear list on load to insure it's empty
        public override void OnLoadVessel()
        {
            base.OnLoadVessel();
            UpdateDriveList();
        }
        //clear list on unload, would otherwise prevent referenced partmodules from being destroyed.
        public override void OnUnloadVessel()
        {
            DisableDrive();
            GameEvents.onVesselStandardModification.Remove(UpdateDriveList);
            DriveList.Clear();
            PartMass.Clear();
        }

        //disables the drive, updates the driveList and send call to PartModules to update their VesselDrive.
        public void UpdateDriveList(Vessel GameEventVessel=null)
        {
            GameEvents.onVesselStandardModification.Remove(UpdateDriveList);
            GameEvents.onVesselStandardModification.Add(UpdateDriveList);
            EChash = PartResourceLibrary.Instance.GetDefinition("ElectricCharge").id;
            Fluxhash = PartResourceLibrary.Instance.GetDefinition("Flux").id;
            DisableDrive();
            DriveList = this.vessel.FindPartModulesImplementing<ModuleMEdrive>();
            foreach (ModuleMEdrive Drive in DriveList)
            {
                Drive.UpdateVesselDrive();
            }
        }
        
        public override void OnStart()
        {
            base.OnStart();
            {
                UpdateDriveList();
            }
        }
        public void FixedUpdate()
        {
            //If vessel has at least one drive, run Static Charge venting code. idk if it's optimised but hopefully it shouldn't completely ruin performance.
            double FluxVentingMultiplier = this.vessel.mainBody.scienceValues.spaceAltitudeThreshold / this.vessel.altitude;
            if (DriveCount != 0)
            {
                if (this.vessel.altitude < this.vessel.mainBody.scienceValues.spaceAltitudeThreshold)
                {
                    if (this.vessel.altitude < 1 || FluxVentingMultiplier > 100) //10 is max FluxVentingMultiplier.
                    {
                        FluxVentingMultiplier = 10;
                    }
                    if (this.vessel.altitude < this.vessel.mainBody.scienceValues.spaceAltitudeThreshold)
                    {
                        double StaticChargeDischarge = (0.008 * DriveCount * FluxVentingMultiplier);
                        double StaticChargeDischageDelta = StaticChargeDischarge * TimeWarp.fixedDeltaTime;
                        this.vessel.RequestResource(this.vessel.Parts[0], Fluxhash, StaticChargeDischageDelta, false);
                    }
                    else
                    {
                        FluxVentingMultiplier = 0;
                    }
                }
            }
            

            if (this.vessel.loaded)
            {
                //part of flux venting code, only needs to run (and only ABLE to run) when loaded.
                if (DriveCount != 0)
                {
                    if (this.vessel.altitude < this.vessel.mainBody.scienceValues.spaceAltitudeThreshold)
                    {
                        if (FluxVentingMultiplier != FluxVentingMultPrevious)
                        {
                            FluxVentingMultPrevious = FluxVentingMultiplier;
                            foreach (ModuleMEdrive Drive in DriveList)
                            {
                                Drive.SCDischargeMultPrevious = FluxVentingMultPrevious;
                            }
                        }
                    }
                }
                        
                //Check for DriveCount Update
                if (DriveList.Count() != DriveCount)
                {
                    DriveCount = DriveList.Count();
                }

                //Check for VesselMass Update
                VesselMass = PartMass.Sum(x => x.Value);
                if (VesselMass != VesselMassPrevious)
                {
                    VesselMassPrevious = VesselMass;
                    foreach (ModuleMEdrive Drive in DriveList)
                    {
                        Drive.VesselMass = VesselMass;
                    }
                }
                //Check for DriveEnergy Update 
                double DriveEnergy = CalcEnergy();
                if (DriveEnergy != DriveConsumptionPrevious)
                {
                    foreach (ModuleMEdrive Drive in DriveList)
                    {
                        Drive.DriveConsumption = DriveEnergy;
                    }
                    DriveConsumptionPrevious = DriveEnergy;


                }
                if (MEdriveActivated)
                {
                    double DriveEnergyDelta = DriveEnergy * TimeWarp.fixedDeltaTime;
                    double ECReturn = this.vessel.RequestResource(this.vessel.Parts[0], EChash, DriveEnergyDelta, false);
                    double SCReturn = this.vessel.RequestResource(this.vessel.Parts[0], Fluxhash, -DriveEnergyDelta, false);
                    if (Math.Abs(DriveEnergyDelta - ECReturn) < 0.01 && Math.Abs(DriveEnergyDelta + SCReturn) < 0.01)
                    {
                        //what to do while the engine is running, example, add static buildup
                        float WarpedMass = (GravWarpPercentage / 100) * VesselMass;
                        if (WarpedMass != WarpedMassPrevious)
                        {
                            WarpedMassPrevious = WarpedMass;
                            foreach (ModuleMEdrive Drive in DriveList)
                            {
                                Drive.WarpedMass = WarpedMass;
                            }
                        }
                    }
                    else
                    {
                        DisableDrive();
                    }
                }
            }
        }
    }
}
