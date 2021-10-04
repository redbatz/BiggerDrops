using BattleTech;
using BattleTech.UI;
using Harmony;
using System;
using System.Collections.Generic;
using UnityEngine;
using BiggerDrops.Data;
using BiggerDrops.Features;

namespace BiggerDrops.Patches
{
    [HarmonyPatch(typeof(AAR_UnitsResult_Screen), "InitializeData")]
  public static class AAR_UnitsResult_Screen_InitializeData {
    static bool Prefix(AAR_UnitsResult_Screen __instance, MissionResults mission, SimGameState sim, Contract contract) {
      try {
        if (CustomUnitsAPI.Detected()) { return true; }
        List<AAR_UnitStatusWidget> UnitWidgets = (List<AAR_UnitStatusWidget>)AccessTools.Field(typeof(AAR_UnitsResult_Screen), "UnitWidgets").GetValue(__instance);
        GameObject nextButton = __instance.transform.FindRecursive("buttonPanel").gameObject;
        nextButton.transform.localPosition = new Vector3(150, 400, 0);

        Transform parent = UnitWidgets[0].transform.parent;
        parent.localPosition = new Vector3(0, 115, 0);
        foreach (AAR_UnitStatusWidget oldWidget in UnitWidgets) {
          oldWidget.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
        }
        GameObject newparent = GameObject.Instantiate(parent.gameObject);
        newparent.transform.parent = parent.parent;
        newparent.name = "newparent";
        newparent.transform.localPosition = new Vector3(0, -325, 0);
        foreach (Transform t in newparent.transform) {
          UnitWidgets.Add(t.gameObject.GetComponent<AAR_UnitStatusWidget>());
        }
        AccessTools.Field(typeof(AAR_UnitsResult_Screen), "UnitWidgets").SetValue(__instance, UnitWidgets);

        List<UnitResult> UnitResults = new List<UnitResult>();
        for (int i = 0; i < 8; i++) {
          if (i < contract.PlayerUnitResults.Count) {
            UnitResults.Add(contract.PlayerUnitResults[i]);
          } else {
            UnitResults.Add(null);
          }
        }
        AccessTools.Field(typeof(AAR_UnitsResult_Screen), "simState").SetValue(__instance, sim);
        AccessTools.Field(typeof(AAR_UnitsResult_Screen), "missionResultParent").SetValue(__instance, mission);
        AccessTools.Field(typeof(AAR_UnitsResult_Screen), "theContract").SetValue(__instance, contract);
        AccessTools.Field(typeof(AAR_UnitsResult_Screen), "numUnits").SetValue(__instance, contract.PlayerUnitResults.Count);
        AccessTools.Field(typeof(AAR_UnitsResult_Screen), "UnitResults").SetValue(__instance, UnitResults);
        __instance.Visible = false;
        __instance.InitializeWidgets();
        return false;
      } catch (Exception e) {
        Logger.LogError(e);
        return true;
      }
    }
  }

  [HarmonyPatch(typeof(AAR_UnitsResult_Screen), "FillInData")]
  public static class AAR_UnitsResult_Screen_FillInData {
    static bool Prefix(AAR_UnitsResult_Screen __instance) {
      try {
        if (CustomUnitsAPI.Detected()) { return true; }
        Contract theContract = (Contract)AccessTools.Field(typeof(AAR_UnitsResult_Screen), "theContract").GetValue(__instance);
        List<AAR_UnitStatusWidget> UnitWidgets = (List<AAR_UnitStatusWidget>)AccessTools.Field(typeof(AAR_UnitsResult_Screen), "UnitWidgets").GetValue(__instance);
        List<UnitResult> UnitResults = (List<UnitResult>)AccessTools.Field(typeof(AAR_UnitsResult_Screen), "UnitResults").GetValue(__instance);
        int experienceEarned = theContract.ExperienceEarned;
        for (int i = 0; i < 8; i++) {
          UnitWidgets[i].SetMechIconValueTextActive(false);
          if (UnitResults[i] != null) {
            UnitWidgets[i].SetNoUnitDeployedOverlayActive(false);
            UnitWidgets[i].FillInData(experienceEarned);
          } else {
            UnitWidgets[i].SetNoUnitDeployedOverlayActive(true);
          }
        }
        AccessTools.Field(typeof(AAR_UnitsResult_Screen), "UnitWidgets").SetValue(__instance, UnitWidgets);
        return false;
      } catch (Exception e) {
        Logger.LogError(e);
        return true;
      }
    }
  }
}