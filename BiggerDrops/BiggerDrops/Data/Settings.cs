using Newtonsoft.Json;
using System.Collections.Generic;
using BattleTech;
using System;

namespace BiggerDrops.Data
{
    public class Settings {
      
        public int MAX_VEHICLE_SLOTS = 8;
        public static readonly string EMPLOYER_LANCE_GUID = "ecc8d4f2-74b4-465d-adf6-84445e5dfc230";
        public int MAX_CU_DROP_SIZE = 12;
        public int MAX_CU_ADDITINAL_MECH_SLOTS = 8;

        public bool debugLog = false;
        public string additionalLanceName = "AI LANCE";
        public bool allowUpgrades = false;
        public bool showAdditionalArgoUpgrades = false;
        public string argoUpgradeName = "Command & Control";
        public string argoUpgradeCategory1Name = "Drop Size";
        public string argoUpgradeCategory2Name = "Mech Control";
        public string argoUpgradeCategory3Name = "Drop Tonnage";
        public int CuInitialVehicles = 0;
        public bool respectFourDropLimit = false;
        public bool limitFlashpointDrop = true;
        public int additinalMechSlots = 4;
        public int additinalPlayerMechSlots = 4;
        public int defaultMaxTonnage = 500;
        
    
    }
}