using Newtonsoft.Json;
using System.Collections.Generic;
using BattleTech;
using System;
using System.Linq;
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
        public static readonly string legacyUpgradeDone = "BiggerDrops_LegacyUpgrade";
        public static readonly int MinCuBays = 3;
        public static readonly int MaxAdditionalMechSlots = 4;
        
        private static StatCollection companyStats;
        private static List<string> SlotOrder = new List<string>();

        public static void FindSlotOrder(List<DropSlotDef> defs)
        {
            Dictionary<int, List<string>> Buckets = new Dictionary<int, List<string>>();
            foreach (DropSlotDef def in defs)
            {
                if (Buckets.ContainsKey(def.Order))
                {
                    Buckets[def.Order].Add(def.Description.Id);
                }
                else
                {
                    List<string> temp = new List<string> { def.Description.Id };
                    Buckets.Add(def.Order, temp);
                }
            }

            SlotOrder = new List<string>();
            foreach (KeyValuePair<int, List<string>> pair in Buckets.OrderBy(i => i.Key))
            {
                foreach (string id in pair.Value)
                {
                    SlotOrder.Add(id);
                }
            }
        }

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
            bool hasCu2 = CustomUnitsAPI.Detected_V2();
            if (BiggerDrops.settings.allowUpgrades)
            {
                if (hasCu2)
                {
                    // one time upgrade of this stat for existing careers
                    if (companyStats.ContainsStatistic(AdditionalMechSlotsStat) && !companyStats.ContainsStatistic(legacyUpgradeDone))
                    {
                        companyStats.AddStatistic(legacyUpgradeDone, 1);
                        companyStats.Int_Add(companyStats.GetStatistic(AdditionalMechSlotsStat), DefaultMechSlots);
                    }
                    if (!companyStats.ContainsStatistic(legacyUpgradeDone))
                    {
                        companyStats.AddStatistic(legacyUpgradeDone, 1);
                    }
                    Dictionary<string, int> SlotCount = new Dictionary<string, int>();
                    foreach (string id in BiggerDrops.settings.CuV2InitialSlots)
                    {
                        if (SlotCount.ContainsKey(id))
                        {
                            SlotCount[id]++;
                        }
                        else
                        {
                            SlotCount.Add(id, 1);
                        }
                    }
                    foreach (string slotId in SlotOrder)
                    {
                        DropSlotDef def = CustomUnitsAPI.GetDropSlotDef(slotId);
                        if (def != null)
                        {
                            if (!companyStats.ContainsStatistic(def.StatName))
                            {
                                int val = 0;
                                if (SlotCount.ContainsKey(slotId))
                                {
                                    val = SlotCount[slotId];
                                }
                                companyStats.AddStatistic(def.StatName, val);
                                
                                Logger.M.WL($"failed to find slotdef: {slotId}");
                            }
                        }
                        else
                        {
                            Logger.M.WL($"failed to find slotdef: {slotId}");
                        }
                    }
                }
                else
                {
                    
                    if (!companyStats.ContainsStatistic(AdditionalMechSlotsStat))
                    {
                        companyStats.AddStatistic(AdditionalMechSlotsStat,
                            Math.Max(Math.Min(MaxAdditionalMechSlots, BiggerDrops.settings.additinalMechSlots), 0));
                    }

                    if (!companyStats.ContainsStatistic(AdditionPlayerMechsStat))
                    {
                        companyStats.AddStatistic(AdditionPlayerMechsStat,
                            Math.Max(Math.Min(MaxAdditionalMechSlots, BiggerDrops.settings.additinalPlayerMechSlots),
                                0));
                    }
                    
                    if (!companyStats.ContainsStatistic(CuVehicleStat))
                    {
                        companyStats.AddStatistic(CuVehicleStat, BiggerDrops.settings.CuInitialVehicles);
                    };
                }

                if (!companyStats.ContainsStatistic(MaxTonnageStat))
                {
                    companyStats.AddStatistic(MaxTonnageStat, 
                        Math.Max(BiggerDrops.settings.defaultMaxTonnage, 0));
                }

                
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

        private static void CuV2NoUpgrades()
        {
            List<List<string>> LanceLayout = new List<List<string>>();
            List<string> currentLance = new List<string>();
            
            foreach (string slot in BiggerDrops.settings.CuV2InitialSlots)
            {
                if (currentLance.Count >= BiggerDrops.settings.CuV2FormationSize)
                {
                    LanceLayout.Add(currentLance);
                    currentLance = new List<string>();
                }
                currentLance.Add(slot);
            }
            CustomUnitsAPI.PushDropLayout(CustomUnitsAPI.BIGGER_DROPS_LAYOUT_ID, LanceLayout, BiggerDrops.settings.CuV2InitialSlots.Count);
        }

        private static void UpdateCULancesV2()
        {
            int UnitCount = 0;
            List<List<string>> LanceLayout = new List<List<string>>();
            List<string> currentLance = new List<string>();
            List<DropSlotDef> separateLance = new List<DropSlotDef>();
            List<DropSlotDef> HotDrop = new List<DropSlotDef>();
            List<List<DropSlotDef>> combinedLances = new List<List<DropSlotDef>>();

            foreach (string slotId in SlotOrder)
            {
                DropSlotDef def = CustomUnitsAPI.GetDropSlotDef(slotId);
                if (def != null)
                {
                    if (def.combineLanceWith.Count > 0)
                    {
                        bool bFound = false;
                        foreach (List<DropSlotDef> cLance in combinedLances)
                        {
                            foreach (DropSlotDef slot in cLance)
                            {
                                if (def.combineLanceWith.Contains(slot.Description.Id))
                                {
                                    bFound = true;
                                    break;
                                }
                            }
                            if (bFound)
                            {
                                cLance.Add(def);
                                break;
                            }
                        }

                        if (!bFound)
                        {
                            List<DropSlotDef> cLance = new List<DropSlotDef>();
                            cLance.Add(def);
                            combinedLances.Add(cLance);
                        }
                    }
                    else
                    {
                        if (def.SeparateLance || def.HotDrop)
                        {
                            if (def.HotDrop)
                            {
                                HotDrop.Add(def);
                            }
                            else
                            {
                                separateLance.Add(def);
                            }
                        }
                        else
                        {
                            int slotCount = companyStats.GetValue<int>(def.StatName);
                            UnitCount += slotCount;
                            for (int i = 0; i < slotCount; i++)
                            {
                                if (currentLance.Count >= BiggerDrops.settings.CuV2FormationSize)
                                {
                                    LanceLayout.Add(currentLance);
                                    currentLance = new List<string>();
                                }

                                currentLance.Add(slotId);
                            }
                        }
                    }
                }
            }

            if (currentLance.Count > 0)
            {
                LanceLayout.Add(currentLance);
            }

            foreach (List<DropSlotDef> cLance in combinedLances)
            {
                currentLance = new List<string>();
                foreach (DropSlotDef def in cLance)
                {
                    int slotCount = companyStats.GetValue<int>(def.StatName);
                    UnitCount += slotCount;
                    for (int i = 0; i < slotCount; i++)
                    {
                        if (currentLance.Count >= BiggerDrops.settings.CuV2FormationSize)
                        {
                            LanceLayout.Add(currentLance);
                            currentLance = new List<string>();
                        }
                        currentLance.Add(def.Description.Id);
                    }
                }
                if (currentLance.Count > 0)
                {
                    LanceLayout.Add(currentLance);
                }
            }
            
            foreach (DropSlotDef def in separateLance)
            {
                currentLance = new List<string>();
                int slotCount = companyStats.GetValue<int>(def.StatName);
                UnitCount += slotCount;
                for (int i = 0; i < slotCount; i++)
                {
                    if (currentLance.Count >= BiggerDrops.settings.CuV2FormationSize)
                    {
                        LanceLayout.Add(currentLance);
                        currentLance = new List<string>();
                    }
                    currentLance.Add(def.Description.Id);
                } 
                if (currentLance.Count > 0)
                {
                    LanceLayout.Add(currentLance);
                }
            }
            
            currentLance = new List<string>();
            foreach (DropSlotDef def in HotDrop)
            {
                
                int slotCount = companyStats.GetValue<int>(def.StatName);
                UnitCount += slotCount;
                for (int i = 0; i < slotCount; i++)
                {
                    if (currentLance.Count >= BiggerDrops.settings.CuV2FormationSize)
                    {
                        LanceLayout.Add(currentLance);
                        currentLance = new List<string>();
                    }
                    currentLance.Add(def.Description.Id);
                } 
                
            }
            if (currentLance.Count > 0)
            {
                LanceLayout.Add(currentLance);
            }
            CustomUnitsAPI.PushDropLayout(CustomUnitsAPI.BIGGER_DROPS_LAYOUT_ID, LanceLayout, UnitCount);
            int iBayCount = Math.Max(MinCuBays, companyStats.GetValue<int>("MechBayPods"));
            CustomUnitsAPI.setMechBayCount(iBayCount);

        }

        public static void UpdateCULances()
        {
            if (CustomUnitsAPI.Detected_V2())
            {
                if (BiggerDrops.settings.allowUpgrades)
                {
                    UpdateCULancesV2();
                }
                else
                {
                    CuV2NoUpgrades();
                }
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