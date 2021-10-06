using Newtonsoft.Json;
using System.Collections.Generic;
using BattleTech;
using System;
using BiggerDrops.Data;

namespace BiggerDrops.Features
{
    public static class DropManager
    {
        public static readonly int DefaultMechSlots = 4;
        public static readonly string EmployerLanceGuid = "ecc8d4f2-74b4-465d-adf6-84445e5dfc230";
        public static readonly string AdditionalMechSlotsStat = "BiggerDrops_AdditionalMechSlots";
        public static readonly string AdditionPlayerMechsStat = "BiggerDrops_AdditionalPlayerMechSlots";
        public static readonly string MaxTonnageStat = "BiggerDrops_MaxTonnage";
        public static readonly string CuVehicleStat = "BiggerDrops_CuVehicleCount";
        public static readonly int MinCuBays = 3;
        public static readonly int MaxAdditionalMechSlots = 4;
        
        private static StatCollection companyStats;

        public static int AdditionalMechSlots()
        {
            if (BiggerDrops.settings.allowUpgrades && companyStats != null)
            {
                int maxSize = CustomUnitsAPI.Detected() ? BiggerDrops.settings.MAX_CU_ADDITINAL_MECH_SLOTS : MaxAdditionalMechSlots;
                int val = companyStats.GetValue<int>(AdditionalMechSlotsStat);
                return val > maxSize ? maxSize : val;
            }

            return Math.Max(Math.Min(MaxAdditionalMechSlots, BiggerDrops.settings.additinalMechSlots), 0);
        }

        public static int AdditionalPlayerMechs()
        {
            if (BiggerDrops.settings.allowUpgrades && companyStats != null)
            {
                int maxSize = CustomUnitsAPI.Detected() ? BiggerDrops.settings.MAX_CU_ADDITINAL_MECH_SLOTS : MaxAdditionalMechSlots;
                int val = companyStats.GetValue<int>(AdditionPlayerMechsStat);
                return val > maxSize ? maxSize : val;
            }

            return Math.Max(Math.Min(MaxAdditionalMechSlots, BiggerDrops.settings.additinalPlayerMechSlots), 0);
        }

        public static int MaxTonnage()
        {
            if (BiggerDrops.settings.allowUpgrades && companyStats != null)
            {
                return companyStats.GetValue<int>(MaxTonnageStat);
            }
            return Math.Max(BiggerDrops.settings.defaultMaxTonnage, 0);
        }
        
        public static int VehicleCount()
        {
            if (BiggerDrops.settings.allowUpgrades && companyStats != null)
            {
                int val = companyStats.GetValue<int>(CuVehicleStat);
                return val > BiggerDrops.settings.MAX_VEHICLE_SLOTS ? BiggerDrops.settings.MAX_VEHICLE_SLOTS : val;
            }
            return BiggerDrops.settings.CuInitialVehicles;
        }

        public static void setCompanyStats(StatCollection stats) {
            companyStats = stats;
            if (BiggerDrops.settings.allowUpgrades)
            {

                if (!companyStats.ContainsStatistic(AdditionalMechSlotsStat))
                {
                    companyStats.AddStatistic(AdditionalMechSlotsStat,
                        Math.Max(Math.Min(MaxAdditionalMechSlots, BiggerDrops.settings.additinalMechSlots), 0)); 
                }
                if (!companyStats.ContainsStatistic(AdditionPlayerMechsStat)) {
                    companyStats.AddStatistic(AdditionPlayerMechsStat, 
                    Math.Max(Math.Min(MaxAdditionalMechSlots, BiggerDrops.settings.additinalPlayerMechSlots), 0)); 
                }

                if (!companyStats.ContainsStatistic(MaxTonnageStat))
                {
                    companyStats.AddStatistic(MaxTonnageStat, 
                        Math.Max(BiggerDrops.settings.defaultMaxTonnage, 0));
                }

                if (!companyStats.ContainsStatistic(CuVehicleStat))
                {
                    companyStats.AddStatistic(CuVehicleStat, BiggerDrops.settings.CuInitialVehicles);
                };
            }
            UpdateCULances();
        }
        
        private static void UpdateCULancesV1() {
            int lanceCount = 1;
            int mCount = AdditionalMechSlots();
            while (mCount > 0)
            {
                lanceCount++;
                mCount -= 4;
            }
            int vCount = VehicleCount();
            while (vCount > 0)
            {
                lanceCount++;
                vCount -= 4;
            }
            int iBayCount = Math.Max(MinCuBays, companyStats.GetValue<int>("MechBayPods"));
            CustomUnitsAPI.setLancesCount(lanceCount);
            CustomUnitsAPI.setLanceData(0, DefaultMechSlots, DefaultMechSlots, false);
            int vStart = 1;
            mCount = AdditionalMechSlots();
            while (mCount > 0)
            {
                CustomUnitsAPI.setLanceData(vStart, DefaultMechSlots, mCount > DefaultMechSlots ? DefaultMechSlots : mCount, false);
                vStart++;
                mCount -= 4;          
            }
            vCount = VehicleCount();
            while (vCount > 0)
            {
                CustomUnitsAPI.setLanceData(vStart, DefaultMechSlots, vCount > DefaultMechSlots ? DefaultMechSlots : vCount, true);
                vStart++;
                vCount -= 4;
            }
            int mechCount = DefaultMechSlots + AdditionalPlayerMechs();
            CustomUnitsAPI.setOverallDeployCount(System.Math.Min(DefaultMechSlots + AdditionalMechSlots() + VehicleCount(), BiggerDrops.settings.MAX_CU_DROP_SIZE));
            // Tanks can fit in either mech or tank slots so count their max drop as as the combine total of slots
            CustomUnitsAPI.playerControl(mechCount, VehicleCount() + mechCount);
            CustomUnitsAPI.setMechBayCount(iBayCount);
        }

        private static void UpdateCULancesV2()
        {
        }

        public static void UpdateCULances()
        {
            if (CustomUnitsAPI.Detected_V2())
            {
                UpdateCULancesV2();
            }
            else
            {
                if (CustomUnitsAPI.Detected())
                {
                    UpdateCULancesV1();
                }
            }
        }

    }
}