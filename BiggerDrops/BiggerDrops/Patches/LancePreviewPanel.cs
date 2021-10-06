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
    [HarmonyPatch(typeof(LancePreviewPanel), "SaveLance")]
    public static class LancePreviewPanel_SaveLance {
        static void Prefix(LancePreviewPanel __instance, LanceDef lanceToSave) {
            Logger.M.TWL(0, "LancePreviewPanel.SaveLance", true);
            Logger.M.WL(0, lanceToSave.ToJSON(), true);
        }
    }
    [HarmonyPatch(typeof(LancePreviewPanel), "SetData")]
    public static class LancePreviewPanel_SetData {
        static void Prefix(LancePreviewPanel __instance, ref int maxUnits) {
            try {
                if (CustomUnitsAPI.Detected()) { return; }
                maxUnits = DropManager.DefaultMechSlots + DropManager.MaxAdditionalMechSlots;
                if (__instance.loadoutSlots.Length >= maxUnits) { return; }
                if (__instance.loadoutSlots.Length < 2) { maxUnits = __instance.loadoutSlots.Length; return; };
                float ydelta = __instance.loadoutSlots[1].GetComponent<RectTransform>().localPosition.y - __instance.loadoutSlots[0].GetComponent<RectTransform>().localPosition.y;
                int addUnits = maxUnits - __instance.loadoutSlots.Length;
                GameObject srcLayout = __instance.loadoutSlots[__instance.loadoutSlots.Length - 1].gameObject;
                List<LanceLoadoutSlot> slots = new List<LanceLoadoutSlot>();
                slots.AddRange(__instance.loadoutSlots);
                for(int t = 0; t < addUnits; ++t) {
                    GameObject nLayout = GameObject.Instantiate(srcLayout,srcLayout.transform.parent);
                    RectTransform rt = nLayout.GetComponent<RectTransform>();
                    Vector3 pos = rt.localPosition;
                    pos.y = srcLayout.GetComponent<RectTransform>().localPosition.y + (t + 1) * ydelta;
                    rt.localPosition = pos;
                    slots.Add(nLayout.GetComponent<LanceLoadoutSlot>());
                }
                __instance.loadoutSlots = slots.ToArray();
            } catch (Exception e) {
                Logger.M.TWL(0, e.ToString());
            }
        }
    }
    
    [HarmonyPatch(typeof(LancePreviewPanel), "OnLanceConfiguratorConfirm")]
    public static class LancePreviewPanel_OnLanceConfiguratorConfirm {
        static void Prefix(LancePreviewPanel __instance) {
            Logger.M.TWL(0, "LancePreviewPanel.OnLanceConfiguratorConfirm", true);
        }
    }
    [HarmonyPatch(typeof(LancePreviewPanel), "OnLanceConfiguratorCancel")]
    public static class LancePreviewPanel_OnLanceConfiguratorCancel {
        static void Prefix(LancePreviewPanel __instance) {
            Logger.M.TWL(0, "LancePreviewPanel.OnLanceConfiguratorCancel", true);
        }
    }
}