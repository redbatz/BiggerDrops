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

        public bool debugLog { get; set; }
        public bool debugLanceLoadout { get; set; }
        public int skirmishMax { get; set; }
        public string additionalLanceName { get; set; }
        public bool allowUpgrades { get; set; }
        public bool showAdditionalArgoUpgrades { get; set; }
        public string argoUpgradeName { get; set; }
        public string argoUpgradeCategory1Name { get; set; }
        public string argoUpgradeCategory2Name { get; set; }
        public string argoUpgradeCategory3Name { get; set; }
        public int CuInitialVehicles { get; set; }
        public bool respectFourDropLimit {get; set;}
        public bool limitFlashpointDrop {get; set;}
        public int additinalMechSlots {get; set;}
        public int additinalPlayerMechSlots {get; set;}

        public int defaultMaxTonnage {get; set;}
        
        public Settings() {
            debugLog = false;
            debugLanceLoadout = false;
            additionalLanceName = "AI LANCE";
            allowUpgrades = false;
            showAdditionalArgoUpgrades = false;
            argoUpgradeName = "Command & Control";
            argoUpgradeCategory1Name = "Drop Size";
            argoUpgradeCategory2Name = "Mech Control";
            argoUpgradeCategory3Name = "Drop Tonnage";
            CuInitialVehicles = 0;
            limitFlashpointDrop = true;
            respectFourDropLimit = false;
            defaultMaxTonnage = 500;
            additinalMechSlots = 4;
            additinalPlayerMechSlots = 4;
        }
    
    }
}