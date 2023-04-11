using BattleTech;
using BattleTech.UI;
using Harmony;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using SVGImporter;
using BiggerDrops.Data;
using BiggerDrops.Features;

namespace BiggerDrops.Patches
{
    [HarmonyPatch(typeof(SGEngineeringScreen), "PopulateUpgradeDictionary")]
    public static class SGEngineeringScreen_PopulateUpgradeDictionary
    {
        public static void Prefix(SGEngineeringScreen __instance)
        {
            if (!BiggerDrops.settings.showAdditionalArgoUpgrades)
            {
                return;
            }
            try
            {
                //This needs to be done by as a prefix to avoid strange bugs with upgrades showing the wrong state
                if (__instance.transform.FindRecursive("BDUpgradePanel") == null)
                {
                    GameObject primelayout = __instance.transform.FindRecursive("uixPrbPanl_SystemsAndSupportPanel").gameObject;
                    GameObject newLayout = GameObject.Instantiate(primelayout);
                    newLayout.transform.parent = primelayout.transform.parent;
                    newLayout.name = "BDUpgradePanel";
                    newLayout.transform.localPosition = new Vector3(-757, 195, 0);
                    TextMeshProUGUI agroUpgradeText = newLayout.transform.FindRecursive("systemsAndSupport_header").gameObject.GetComponent<TextMeshProUGUI>();
                    agroUpgradeText.text = BiggerDrops.settings.argoUpgradeName;
                    TextMeshProUGUI upgrade1Text = newLayout.transform.FindRecursive("text_powerSystem").gameObject.GetComponent<TextMeshProUGUI>();
                    upgrade1Text.text = BiggerDrops.settings.argoUpgradeCategory1Name;
                    TextMeshProUGUI upgrade2Text = newLayout.transform.FindRecursive("text_structureSystem").gameObject.GetComponent<TextMeshProUGUI>();
                    upgrade2Text.text = BiggerDrops.settings.argoUpgradeCategory2Name;
                    TextMeshProUGUI upgrade3Text = newLayout.transform.FindRecursive("text_driveeSystem").gameObject.GetComponent<TextMeshProUGUI>();
                    upgrade3Text.text = BiggerDrops.settings.argoUpgradeCategory3Name;

                    GameObject driverPipSlots = newLayout.transform.FindRecursive("drivePipSlots").gameObject;
                    driverPipSlots.name = "BDDropTonnage";
                    GameObject structurePipSlots = newLayout.transform.FindRecursive("structurePipSlots").gameObject;
                    structurePipSlots.name = "BDMechControl";
                    GameObject powerPipSlots = newLayout.transform.FindRecursive("powerPipSlots").gameObject;
                    powerPipSlots.name = "BDMechDrops";
                    if (BiggerDrops.settings.enableRTExtendedCategories)
                    {
                        TextMeshProUGUI upgrade4Text = newLayout.transform.FindRecursive("text_habitatSystem").gameObject.GetComponent<TextMeshProUGUI>();
                        upgrade4Text.text = BiggerDrops.settings.argoUpgradeCategoryRT1Name;
                        GameObject rtExtra1 = newLayout.transform.FindRecursive("habitatPipSlots").gameObject;
                        rtExtra1.name = "RTExtra1";

                        GameObject combatLayout = __instance.transform.FindRecursive("uixPrfPanlcombatAndDeployment").gameObject;
                        combatLayout.transform.localPosition = new Vector3(-290, -32, 0);
                        GameObject mechBays = combatLayout.transform.FindRecursive("MechBays").gameObject;
                        GameObject rtExtra2Parent = GameObject.Instantiate(mechBays);
                        rtExtra2Parent.transform.parent = mechBays.transform.parent;
                        TextMeshProUGUI rtExtra2Text = rtExtra2Parent.transform.FindRecursive("text_mechBays").gameObject.GetComponent<TextMeshProUGUI>();
                        rtExtra2Text.text = BiggerDrops.settings.argoUpgradeCategoryRT2Name;
                        GameObject rtExtra2 = rtExtra2Parent.transform.FindRecursive("mechBayPipSlots").gameObject;
                        rtExtra2.name = "RTExtra2";
                    }
                    else
                    {
                        GameObject habitat = newLayout.transform.FindRecursive("HabitatSystem").gameObject;
                        habitat.name = "BDHabitat";
                        habitat.SetActive(false);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e);
            }
        }
    }

    [HarmonyPatch(typeof(SGEngineeringScreen), "CleanUpAllUpgradePips")]
    public static class SGEngineeringScreen_CleanUpAllUpgradePips
    {
        public static void Postfix(SGEngineeringScreen __instance)
        {
            try
            {
                if (__instance.transform.FindRecursive("BDUpgradePanel") != null)
                {
                    GameObject primelayout = __instance.transform.FindRecursive("BDUpgradePanel").gameObject;
                    List<SGEngineeringShipUpgradePip> engineeringShipUpgradePipList = new List<SGEngineeringShipUpgradePip>();
                    GameObject driverPipSlots = primelayout.transform.FindRecursive("BDDropTonnage").gameObject;
                    GameObject structurePipSlots = primelayout.transform.FindRecursive("BDMechControl").gameObject;
                    GameObject powerPipSlots = primelayout.transform.FindRecursive("BDMechDrops").gameObject;
                    engineeringShipUpgradePipList.AddRange((IEnumerable<SGEngineeringShipUpgradePip>)driverPipSlots.GetComponentsInChildren<SGEngineeringShipUpgradePip>());
                    engineeringShipUpgradePipList.AddRange((IEnumerable<SGEngineeringShipUpgradePip>)structurePipSlots.GetComponentsInChildren<SGEngineeringShipUpgradePip>());
                    engineeringShipUpgradePipList.AddRange((IEnumerable<SGEngineeringShipUpgradePip>)powerPipSlots.GetComponentsInChildren<SGEngineeringShipUpgradePip>());
                    if (BiggerDrops.settings.enableRTExtendedCategories)
                    {
                        GameObject rtExtra1 = primelayout.transform.FindRecursive("RTExtra1").gameObject;
                        engineeringShipUpgradePipList.AddRange((IEnumerable<SGEngineeringShipUpgradePip>)rtExtra1.GetComponentsInChildren<SGEngineeringShipUpgradePip>());

                        GameObject combatLayout = __instance.transform.FindRecursive("uixPrfPanlcombatAndDeployment").gameObject;
                        GameObject rtExtra2 = combatLayout.transform.FindRecursive("RTExtra2").gameObject;
                        engineeringShipUpgradePipList.AddRange((IEnumerable<SGEngineeringShipUpgradePip>)rtExtra2.GetComponentsInChildren<SGEngineeringShipUpgradePip>());


                    }
                    List<ShipModuleUpgrade> available = (List<ShipModuleUpgrade>)AccessTools.Field(typeof(SGEngineeringScreen), "AvailableUpgrades").GetValue(__instance);
                    List<ShipModuleUpgrade> purchased = (List<ShipModuleUpgrade>)AccessTools.Field(typeof(SGEngineeringScreen), "PurchasedUpgrades").GetValue(__instance);
                    UIManager uiManager = (UIManager)AccessTools.Field(typeof(SGEngineeringScreen), "uiManager").GetValue(__instance);
                    foreach (SGEngineeringShipUpgradePip engineeringShipUpgradePip in engineeringShipUpgradePipList)
                    {
                        string id = "uixPrfIndc_SIM_argoUpgradePipUnavailable-element";

                        if (available.Contains(engineeringShipUpgradePip.UpgradeModule))
                            id = "uixPrfIndc_SIM_argoUpgradePipAvailable-element";
                        else if (purchased.Contains(engineeringShipUpgradePip.UpgradeModule))
                            id = "uixPrfIndc_SIM_argoUpgradePip-element";
                        uiManager.dataManager.PoolGameObject(id, engineeringShipUpgradePip.gameObject);
                    }

                }
            }
            catch (Exception e)
            {
                Logger.LogError(e);
            }
        }
    }

    [HarmonyPatch(typeof(SGEngineeringScreen), "ClearUpgradePips")]
    public static class SGEngineeringScreen_ClearUpgradePips
    {
        public static void Postfix(SGEngineeringScreen __instance)
        {
            try
            {
                if (__instance.transform.FindRecursive("BDUpgradePanel") != null)
                {
                    GameObject primelayout = __instance.transform.FindRecursive("BDUpgradePanel").gameObject;
                    List<GameObject> engineeringShipUpgradePipList = new List<GameObject>();
                    GameObject driverPipSlots = primelayout.transform.FindRecursive("BDDropTonnage").gameObject;
                    foreach (Transform transform in driverPipSlots.transform)
                    {
                        if ((UnityEngine.Object)transform.gameObject.GetComponent<SGEngineeringShipUpgradePip>() != (UnityEngine.Object)null)
                            engineeringShipUpgradePipList.Add(transform.gameObject);
                    }
                    GameObject structurePipSlots = primelayout.transform.FindRecursive("BDMechControl").gameObject;
                    foreach (Transform transform in structurePipSlots.transform)
                    {
                        if ((UnityEngine.Object)transform.gameObject.GetComponent<SGEngineeringShipUpgradePip>() != (UnityEngine.Object)null)
                            engineeringShipUpgradePipList.Add(transform.gameObject);
                    }
                    GameObject powerPipSlots = primelayout.transform.FindRecursive("BDMechDrops").gameObject;
                    foreach (Transform transform in powerPipSlots.transform)
                    {
                        if ((UnityEngine.Object)transform.gameObject.GetComponent<SGEngineeringShipUpgradePip>() != (UnityEngine.Object)null)
                            engineeringShipUpgradePipList.Add(transform.gameObject);
                    }
                    if (BiggerDrops.settings.enableRTExtendedCategories)
                    {
                        GameObject rtExtra1 = primelayout.transform.FindRecursive("RTExtra1").gameObject;
                        foreach (Transform transform in rtExtra1.transform)
                        {
                            if ((UnityEngine.Object)transform.gameObject.GetComponent<SGEngineeringShipUpgradePip>() != (UnityEngine.Object)null)
                                engineeringShipUpgradePipList.Add(transform.gameObject);
                        }

                        GameObject combatLayout = __instance.transform.FindRecursive("uixPrfPanlcombatAndDeployment").gameObject;
                        GameObject rtExtra2 = combatLayout.transform.FindRecursive("RTExtra2").gameObject;
                        foreach (Transform transform in rtExtra2.transform)
                        {
                            if ((UnityEngine.Object)transform.gameObject.GetComponent<SGEngineeringShipUpgradePip>() != (UnityEngine.Object)null)
                                engineeringShipUpgradePipList.Add(transform.gameObject);
                        }
                    }
                    List<ShipModuleUpgrade> available = (List<ShipModuleUpgrade>)AccessTools.Field(typeof(SGEngineeringScreen), "AvailableUpgrades").GetValue(__instance);
                    List<ShipModuleUpgrade> purchased = (List<ShipModuleUpgrade>)AccessTools.Field(typeof(SGEngineeringScreen), "PurchasedUpgrades").GetValue(__instance);
                    SimGameState simGame = (SimGameState)AccessTools.Property(typeof(SGEngineeringScreen), "simState").GetValue(__instance);
                    engineeringShipUpgradePipList.ForEach((Action<GameObject>)(item =>
                    {
                        string id = "uixPrfIndc_SIM_argoUpgradePipUnavailable-element";
                        ShipModuleUpgrade upgradeModule = item.GetComponent<SGEngineeringShipUpgradePip>().UpgradeModule;
                        if (available.Contains(upgradeModule))
                            id = "uixPrfIndc_SIM_argoUpgradePipAvailable-element";
                        else if (purchased.Contains(upgradeModule))
                            id = "uixPrfIndc_SIM_argoUpgradePip-element";
                        simGame.DataManager.PoolGameObject(id, item);
                    }));
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e);
            }
        }
    }

    [HarmonyPatch(typeof(SGEngineeringScreen), "AddUpgradePip")]
    public static class SGEngineeringScreen_AddUpgradePip
    {
        public static void UpgradeSelected(this SGEngineeringScreen screen, ShipModuleUpgrade upgrade)
        {
            AccessTools.Method(typeof(SGEngineeringScreen), "OnUpgradeSelected").Invoke(screen, new object[] { upgrade });
        }
        public static bool Prefix(SGEngineeringScreen __instance, ShipModuleUpgrade upgrade)
        {
            if (!BiggerDrops.settings.showAdditionalArgoUpgrades)
            {
                return true;
            }
            try
            {
                //Todo: upgrades at or below this are vanilla
                if (upgrade.ShipUpgradeCategoryValue.IsVanilla /*upgrade.ShipUpgradeCategoryValue.ID <= ShipUpgradeCategoryEnumeration.GetShipUpgradeCategoryByName("TRAINING").ID*/)
                {
                    return true;
                }

                if (__instance.transform.FindRecursive("BDUpgradePanel") != null)
                {
                    GameObject primelayout = __instance.transform.FindRecursive("BDUpgradePanel").gameObject;
                    Transform BDMechDrops = primelayout.transform.FindRecursive("BDMechDrops");
                    Transform BDMechControl = primelayout.transform.FindRecursive("BDMechControl");
                    Transform BDDropTonnage = primelayout.transform.FindRecursive("BDDropTonnage");
                    Transform RTExtra1 = null;
                    Transform RTExtra2 = null;
                    if (BiggerDrops.settings.enableRTExtendedCategories)
                    {
                        RTExtra1 = primelayout.transform.FindRecursive("RTExtra1");
                        GameObject combatLayout = __instance.transform.FindRecursive("uixPrfPanlcombatAndDeployment").gameObject;
                        RTExtra2 = combatLayout.transform.FindRecursive("RTExtra2");
                    }
                    List<ShipModuleUpgrade> available = (List<ShipModuleUpgrade>)AccessTools.Field(typeof(SGEngineeringScreen), "AvailableUpgrades").GetValue(__instance);
                    List<ShipModuleUpgrade> purchased = (List<ShipModuleUpgrade>)AccessTools.Field(typeof(SGEngineeringScreen), "PurchasedUpgrades").GetValue(__instance);
                    SimGameState simGame = (SimGameState)AccessTools.Property(typeof(SGEngineeringScreen), "simState").GetValue(__instance);
                    UIManager uiManager = (UIManager)AccessTools.Field(typeof(SGEngineeringScreen), "uiManager").GetValue(__instance);
                    Transform parent;
                    if (upgrade.ShipUpgradeCategoryValue.Name == "BDDropTonnage")
                    {
                        parent = BDDropTonnage;
                    }
                    else if (upgrade.ShipUpgradeCategoryValue.Name == "BDMechControl")
                    {
                        parent = BDMechControl;
                    }
                    else if (upgrade.ShipUpgradeCategoryValue.Name == "BDMechDrops")
                    {
                        parent = BDMechDrops;
                    }
                    else if (RTExtra1 != null && upgrade.ShipUpgradeCategoryValue.Name == "RTExtra1")
                    {
                        parent = RTExtra1;
                    }
                    else if (RTExtra2 != null && upgrade.ShipUpgradeCategoryValue.Name == "RTExtra2")
                    {
                        parent = RTExtra2;
                    }
                    else
                    {
                        Debug.LogWarning((object)string.Format("Invalid location ({0}) for ship module {1}", (object)upgrade.Location, (object)upgrade.Description.Id));
                        return false;
                    }
                    string id = "uixPrfIndc_SIM_argoUpgradePipUnavailable-element";
                    if (available.Contains(upgrade))
                        id = "uixPrfIndc_SIM_argoUpgradePipAvailable-element";
                    else if (purchased.Contains(upgrade))
                        id = "uixPrfIndc_SIM_argoUpgradePip-element";
                    SGEngineeringShipUpgradePip component = uiManager.dataManager.PooledInstantiate(id, BattleTechResourceType.UIModulePrefabs, new Vector3?(), new Quaternion?(), parent).GetComponent<SGEngineeringShipUpgradePip>();
                    component.transform.localScale = Vector3.one;
                    component.SetUpgadeModule(upgrade);
                    simGame.RequestItem<SVGAsset>(upgrade.Description.Icon, new Action<SVGAsset>(component.SetIcon), BattleTechResourceType.SVGAsset);
                    component.OnModuleSelected.RemoveAllListeners();
                    component.OnModuleSelected.AddListener(new UnityAction<ShipModuleUpgrade>(__instance.UpgradeSelected));
                }

            }
            catch (Exception e)
            {
                Logger.LogError(e);
            }
            return false;
        }
    }
}