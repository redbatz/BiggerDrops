using BattleTech;
using BattleTech.UI;
using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using BiggerDrops.Data;
using BiggerDrops.Features;

namespace BiggerDrops.Patches
{
    [HarmonyPatch(typeof(LanceConfiguratorPanel), "SetData")]
  public static class LanceConfiguratorPanel_SetData {
    public static void UpdateSlotsCount(this LanceConfiguratorPanel panel, int maxUnits) {
      Logger.M.TWL(0, "LanceConfiguratorPanel.UpdateSlotsCount "+maxUnits);
      try {
        if (CustomUnitsAPI.Detected()) { return; }
        LanceLoadoutSlot[] loadoutSlots = (LanceLoadoutSlot[])AccessTools.Field(typeof(LanceConfiguratorPanel), "loadoutSlots").GetValue(panel);
        if (maxUnits <= loadoutSlots.Length) {
          Logger.M.TWL(1, "already fixed");
          return;
        };
        Transform newLayoutTransform = panel.transform.FindRecursive("AlliedSlots");
        GameObject newLayout;
        GameObject primelayout = panel.transform.FindRecursive("uixPrfPanel_LC_LanceSlots-Widget-MANAGED").gameObject;
        if (newLayoutTransform == null)
        {
            
            newLayout = GameObject.Instantiate(primelayout);
            newLayout.transform.parent = primelayout.transform.parent;
            newLayout.name = "AlliedSlots";
        }
        else
        {
            newLayout = newLayoutTransform.gameObject;
        }
        GameObject slot1 = newLayout.transform.FindRecursive("lanceSlot1").gameObject;
        GameObject slot2 = newLayout.transform.FindRecursive("lanceSlot2").gameObject;
        GameObject slot3 = newLayout.transform.FindRecursive("lanceSlot3").gameObject;
        GameObject slot4 = newLayout.transform.FindRecursive("lanceSlot4").gameObject;
        primelayout.transform.FindRecursive("simbg").gameObject.SetActive(false);
        newLayout.transform.FindRecursive("simbg").gameObject.SetActive(false);
        newLayout.transform.FindRecursive("layout-lanceRating").gameObject.SetActive(false);
        newLayout.transform.FindRecursive("lanceSlotHeader-Campaign").gameObject.SetActive(true);
        newLayout.transform.FindRecursive("txt-unreadyLanceError").gameObject.SetActive(false);
        TextMeshProUGUI aiText = newLayout.transform.FindRecursive("label-readyLanceHeading").gameObject.GetComponent<TextMeshProUGUI>();
        aiText.text = BiggerDrops.settings.additionalLanceName;
        primelayout.transform.position = new Vector3(650, 315, primelayout.transform.position.z);
        primelayout.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
        newLayout.transform.position = new Vector3(650, 83, primelayout.transform.position.z);
        newLayout.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);

        GameObject deployButton = panel.transform.FindRecursive("DeployBttn-layout").gameObject;
        deployButton.transform.position = new Vector3(1675, 175, deployButton.transform.position.z);

        //LanceLoadoutSlot[] loadoutSlots = (LanceLoadoutSlot[])AccessTools.Field(typeof(LanceConfiguratorPanel), "loadoutSlots").GetValue(panel);
        List<LanceLoadoutSlot> list = loadoutSlots.ToList();
        int addUnits = maxUnits - DropManager.DefaultMechSlots;
        for (int i =0; i < BiggerDrops.baysAlreadyAdded; i++)
        {
            list.RemoveAt(DropManager.DefaultMechSlots + i);
        }
        if (addUnits > 0) { list.Add(slot1.GetComponent<LanceLoadoutSlot>()); }
        if (addUnits > 1) { list.Add(slot2.GetComponent<LanceLoadoutSlot>()); }
        if (addUnits > 2) { list.Add(slot3.GetComponent<LanceLoadoutSlot>()); }
        if (addUnits > 3) { list.Add(slot4.GetComponent<LanceLoadoutSlot>()); }
        loadoutSlots = list.ToArray<LanceLoadoutSlot>();
        AccessTools.Field(typeof(LanceConfiguratorPanel), "loadoutSlots").SetValue(panel, loadoutSlots);

        float[] slotMaxTonnages = (float[])AccessTools.Field(typeof(LanceConfiguratorPanel), "slotMaxTonnages").GetValue(panel);
        float[] slotMinTonnages = (float[])AccessTools.Field(typeof(LanceConfiguratorPanel), "slotMinTonnages").GetValue(panel);
        List<float> listMaxTonnages = slotMaxTonnages.ToList();
        List<float> listMinTonnages = slotMinTonnages.ToList();
        for (int i = 0; i < BiggerDrops.baysAlreadyAdded; i++)
        {
            listMaxTonnages.RemoveAt(DropManager.DefaultMechSlots + i);
            listMinTonnages.RemoveAt(DropManager.DefaultMechSlots + i);
        }
        if (addUnits > 0) { listMaxTonnages.Add(-1); }
        if (addUnits > 1) { listMaxTonnages.Add(-1); }
        if (addUnits > 2) { listMaxTonnages.Add(-1); }
        if (addUnits > 3) { listMaxTonnages.Add(-1); }
        if (addUnits > 0) { listMinTonnages.Add(-1); }
        if (addUnits > 1) { listMinTonnages.Add(-1); }
        if (addUnits > 2) { listMinTonnages.Add(-1); }
        if (addUnits > 3) { listMinTonnages.Add(-1); }
        slotMaxTonnages = listMaxTonnages.ToArray<float>();
        slotMinTonnages = listMinTonnages.ToArray<float>();
        AccessTools.Field(typeof(LanceConfiguratorPanel), "slotMaxTonnages").SetValue(panel, slotMaxTonnages);
        AccessTools.Field(typeof(LanceConfiguratorPanel), "slotMinTonnages").SetValue(panel, slotMinTonnages);
        BiggerDrops.baysAlreadyAdded = addUnits;
        Logger.M.TWL(0, "Skirmish UI fixed");
      } catch (Exception e) {
        Logger.M.TWL(0, e.ToString());
      }
    }
    static void Prefix(LanceConfiguratorPanel __instance, ref int maxUnits, Contract contract) {
      try {
        if (CustomUnitsAPI.Detected()) { return; }
        if (contract != null) {
          maxUnits = DropManager.DefaultMechSlots + DropManager.AdditionalMechSlots();
          __instance.UpdateSlotsCount(maxUnits);
            if(contract.Override != null)
            {
                if (contract.IsFlashpointContract | contract.IsFlashpointCampaignContract)
                {
                    if (BiggerDrops.settings.limitFlashpointDrop)
                    {
                        maxUnits = Math.Min(4, contract.Override.maxNumberOfPlayerUnits);
                    }
                }
                if (BiggerDrops.settings.respectFourDropLimit)
                {
                    if (contract.Override.maxNumberOfPlayerUnits != -1)
                    {
                        maxUnits = contract.Override.maxNumberOfPlayerUnits;
                    }
                }
                else
                {
                    if (contract.Override.maxNumberOfPlayerUnits != 4)
                    {
                        maxUnits = contract.Override.maxNumberOfPlayerUnits;
                    }
                }
            }
         } else {
          maxUnits = DropManager.DefaultMechSlots + DropManager.MaxAdditionalMechSlots;
          BiggerDrops.baysAlreadyAdded = 0;
          __instance.UpdateSlotsCount(maxUnits);
          //SkirmishUIFix(__instance,maxUnits);
        }
      } catch (Exception e) {
        Logger.M.TWL(0, e.ToString());
      }
    }

    static void Postfix(LanceConfiguratorPanel __instance, ref int maxUnits, Contract contract) {
      Logger.M.TWL(0, "LanceConfiguratorPanel.SetData postfix "+maxUnits);
      try {
        if (contract == null) {
          Logger.M.WL(1, "contract is null");
          //__instance.lanceMaxTonnage = BiggerDrops.settings.defaultMaxTonnage;
          return;
        } else
        if (contract.Override.lanceMaxTonnage == -1) {
          __instance.lanceMaxTonnage = DropManager.MaxTonnage();
        }
      } catch (Exception e) {
        Logger.LogError(e);
      }
    }
  }
  [HarmonyPatch(typeof(LanceConfiguratorPanel), "CreateLanceDef")]
  public static class LanceConfiguratorPanel_CreateLanceDef {
    static void Prefix(LanceConfiguratorPanel __instance, string lanceId) {
      LanceLoadoutSlot[] loadoutSlots = (LanceLoadoutSlot[])AccessTools.Field(typeof(LanceConfiguratorPanel), "loadoutSlots").GetValue(__instance);
      Logger.M.TWL(0, "LanceConfiguratorPanel.CreateLanceDef " + lanceId + " slots:" + loadoutSlots.Length, true);
    }
    static void Postfix(LanceConfiguratorPanel __instance, string lanceId, ref LanceDef __result) {
      LanceLoadoutSlot[] loadoutSlots = (LanceLoadoutSlot[])AccessTools.Field(typeof(LanceConfiguratorPanel), "loadoutSlots").GetValue(__instance);
      Logger.M.TWL(0, "LanceConfiguratorPanel.CreateLanceDef result:", true);
      Logger.M.WL(0, __result.ToJSON());
    }
  }
  
  [HarmonyPatch(typeof(LanceConfiguratorPanel), "CreateLanceConfiguration")]
  public static class LanceConfiguratorPanel_CreateLanceConfiguration {
    static bool Prefix(LanceConfiguratorPanel __instance, ref LanceConfiguration __result) {
      try {
        return false;
      } catch (Exception e) {
        Logger.LogError(e);
        return false;
      }
    }

    static void Postfix(LanceConfiguratorPanel __instance, ref LanceConfiguration __result, LanceLoadoutSlot[] ___loadoutSlots) {
      try {
        if (CustomUnitsAPI.Detected()) { return; }
        Fields.callsigns.Clear();
        LanceConfiguration lanceConfiguration = new LanceConfiguration();
        for (int i = 0; i < ___loadoutSlots.Length; i++) {
          LanceLoadoutSlot lanceLoadoutSlot = ___loadoutSlots[i];
          MechDef mechDef = null;
          PilotDef pilotDef = null;
          if (lanceLoadoutSlot.SelectedMech != null) {
            mechDef = lanceLoadoutSlot.SelectedMech.MechDef;
          }
          if (lanceLoadoutSlot.SelectedPilot != null) {
            pilotDef = lanceLoadoutSlot.SelectedPilot.Pilot.pilotDef;
          }
          if (mechDef != null && pilotDef != null) {
            if (i < DropManager.DefaultMechSlots) {
              lanceConfiguration.AddUnit(__instance.playerGUID, mechDef, pilotDef);
            } else {
              //if (i >= BiggerDrops.settings.additinalMechSlots + Settings.DEFAULT_MECH_SLOTS) { break; }
              Logger.M.TWL(0, "LanceConfiguratorPanel.CreateLanceConfiguration. Index:" + i + " additional slots border:" + (DropManager.AdditionalMechSlots() + DropManager.DefaultMechSlots) + " player slots border:" + (DropManager.AdditionalPlayerMechs() + DropManager.DefaultMechSlots));
              if (i >= DropManager.AdditionalPlayerMechs() + DropManager.DefaultMechSlots) {
                Fields.callsigns.Add(pilotDef.Description.Callsign);
                //EMPLOYER ID
                Logger.M.WL(1, "adding to employer lance " + Settings.EMPLOYER_LANCE_GUID + " mech:" + mechDef.Description.Id + " pilot:" + pilotDef.Description.Id);
                lanceConfiguration.AddUnit(Settings.EMPLOYER_LANCE_GUID, mechDef, pilotDef);
              } else {
                Logger.M.WL(1, "adding to player lance " + __instance.playerGUID + " mech:" + mechDef.Description.Id + " pilot:" + pilotDef.Description.Id);
                lanceConfiguration.AddUnit(__instance.playerGUID, mechDef, pilotDef);
              }
            }
          }
        }
        __result = lanceConfiguration;
      } catch (Exception e) {
        Logger.LogError(e);
      }
    }
  }
}