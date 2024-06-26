﻿using System;
using UnityEngine;

namespace MassEffectDrivePlugin
{
    public static class MassHandler
    {
        //NOTE 05/05/2024 : Unused, can be removed. Just keeping around as a memo of how some stuff works.
        public static float CalculateCurrentMass(this Part part)
        {
            if ((part.physicalSignificance == Part.PhysicalSignificance.FULL) && (part.rb != null))
            {
                return part.rb.mass;
            }
            return 0;
        }
        public static bool HasPhysics(this Part part)
        {
            if (part.physicalSignificance == Part.PhysicalSignificance.FULL)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static float GetDryMass(this Part part)
        {
            return part.CalculateCurrentMass() - part.resourceMass;
        }
        public static float GetWetMass(this Part part)
        {
            float mass = part.GetDryMass();
            for (int index = 0; index < part.Resources.Count; ++index)
            {
                PartResource partResource = part.Resources[index];
                mass += (float)partResource.maxAmount * partResource.info.density;
            }

            return mass;
        }
        public static float GetRessourceMass(this Part part)
        {
            float mass = 0;
            for (int index = 0; index < part.Resources.Count; ++index)
            {
                PartResource partResource = part.Resources[index];
                mass += (float)partResource.maxAmount * partResource.info.density;
            }
            return mass;
        }

        public static float GetVesselDryMass(this Vessel vessel)
        {
            float mass = 0;
            foreach (Part p in vessel.parts)
            {
                mass += p.GetDryMass();
            }
            return mass;
        }
        public static float GetVesselWetMass(this Vessel vessel)
        {
            float mass = 0;
            foreach (Part p in vessel.parts)
            {
                mass += p.GetWetMass();
            }
            return mass;
        }
    }
}
