using BattleTech;
using Harmony;
using HBS.Collections;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace BiggerDrops {
  public class DropDescriptionDef {
    public string Id { get; set; }
    public string Name { get; set; }
    public string Details { get; set; }
    public string Icon { get; set; }
  }
  public class DropSlotDef {
    public DropDescriptionDef Description { get; set; } = new DropDescriptionDef();
    public bool Disabled { get; set; } = false; // slot is disabled
    public bool PlayerControl { get; set; } = true; // slot is under player control 
    public float difficultyWeight { get; set; } = 1.0f; // ? weight used by BD ?
  }
  public static class CustomUnitsAPI {
    private static bool CustomUnitsAPI_detected = false;
    private static bool CustomUnitsAPI_v2_detected = false;
    private static MethodInfo m_setLancesCount = null;
    private static MethodInfo m_setLanceData = null;
    private static MethodInfo m_setOverallDeployCount = null;
    private static MethodInfo m_playerControl = null;
    private static MethodInfo m_setMechBayCount = null;
    private static MethodInfo m_PushDropLayout = null;
    private static Dictionary<string, DropSlotDef> dropSlotTypes = new Dictionary<string, DropSlotDef>();
    public static readonly string FALLBACK_DROP_SLOT_TYPE_NAME = "fallback_slot";
    public static readonly string BIGGER_DROPS_LAYOUT_ID = "bigger_drops_layout_id";
    public static void Register(this DropSlotDef def) {
      if (dropSlotTypes.ContainsKey(def.Description.Id)) {
        dropSlotTypes[def.Description.Id] = def;
      } else {
        dropSlotTypes.Add(def.Description.Id, def);
      }
    }
    public static bool Detected() { return CustomUnitsAPI_detected; }
    public static bool Detected_V2() { return CustomUnitsAPI_v2_detected; }
    public static void setLancesCount(int count) { if (m_setLancesCount != null) { m_setLancesCount.Invoke(null, new object[] { count }); }; }
    public static void setLanceData(int lanceid, int size, int allow, bool is_vehicle) { if (m_setLanceData != null) { m_setLanceData.Invoke(null, new object[] { lanceid, size, allow, is_vehicle }); }; }
    public static void setOverallDeployCount(int count) { if (m_setOverallDeployCount != null) { m_setOverallDeployCount.Invoke(null, new object[] { count }); }; }
    public static void playerControl(int mechs, int vehicles) { if (m_playerControl != null) { m_playerControl.Invoke(null, new object[] { mechs, vehicles }); }; }
    public static void setMechBayCount(int count) { if (m_setMechBayCount != null) { m_setMechBayCount.Invoke(null, new object[] { count }); }; }
    public static void PushDropLayout(string id, List<List<string>> layout, int maxUnits) { m_PushDropLayout.Invoke(null, new object[] { id, layout, maxUnits }); }
    public static void CustomUnitsDetected() {
      Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
      Logger.M.TWL(0, "CustomUnitsAPI.CustomUnitsDetected");
      foreach (Assembly assembly in assemblies) {
        //Logger.M.WL(1, assembly.FullName);
        if (assembly.FullName.StartsWith("CustomUnits")) {
          Logger.M.WL(1, assembly.FullName);
          Type helperType = assembly.GetType("CustomUnits.CustomLanceHelper");
          if(helperType != null) {
            m_setLancesCount = helperType.GetMethod("setLancesCount", BindingFlags.Static | BindingFlags.Public);
            if (m_setLancesCount == null) { Logger.M.WL(2, "setLancesCount not found"); } else { Logger.M.WL(2, "setLancesCount found"); }
            m_setLanceData = helperType.GetMethod("setLanceData", BindingFlags.Static | BindingFlags.Public);
            if (m_setLanceData == null) { Logger.M.WL(2, "setLanceData not found"); } else { Logger.M.WL(2, "setLanceData found"); }
            m_setOverallDeployCount = helperType.GetMethod("setOverallDeployCount", BindingFlags.Static | BindingFlags.Public);
            if (m_setOverallDeployCount == null) { Logger.M.WL(2, "setOverallDeployCount not found"); } else { Logger.M.WL(2, "setOverallDeployCount found"); }
            m_playerControl = helperType.GetMethod("playerControl", BindingFlags.Static | BindingFlags.Public);
            if (m_playerControl == null) { Logger.M.WL(2, "playerControl not found"); } else { Logger.M.WL(2, "playerControl found"); }
            m_setMechBayCount = helperType.GetMethod("BaysCount", BindingFlags.Static | BindingFlags.Public);
            if (m_setMechBayCount == null) { Logger.M.WL(2, "BaysCount not found"); } else { Logger.M.WL(2, "BaysCount found"); }
            m_PushDropLayout = helperType.GetMethod("PushDropLayout", BindingFlags.Static | BindingFlags.Public);
            if(m_PushDropLayout == null) {
              Logger.M.WL(2, "PushDropLayout not found");
            } else {
              Logger.M.WL(2, "PushDropLayout found");
              CustomUnitsAPI_v2_detected = true;
            }
            CustomUnitsAPI_detected = true;
            break;
          }
        }
      }
      Logger.M.WL(1, "CU API detected:" + Detected() + " CU API v2 detected:" + Detected_V2());
    }

  }
  public class BiggerDrops {
    internal static string ModDirectory;
    public static Settings settings;
    public static int baysAlreadyAdded = 0;
    public static void FinishedLoading(List<string> loadOrder, Dictionary<string, Dictionary<string, VersionManifestEntry>> customResources) {
      Logger.M.TWL(0, "FinishedLoading", true);
      try {
        foreach (string name in loadOrder) { if (name == "CustomUnits") { CustomUnitsAPI.CustomUnitsDetected(); break; }; }
        foreach (var customResource in customResources) {
          Logger.M.TWL(0, "customResource:" + customResource.Key);
          if (customResource.Key == nameof(DropSlotDef)) {
            foreach (var custItem in customResource.Value) {
              try {
                Logger.M.WL(1, "Path:" + custItem.Value.FilePath);
                DropSlotDef def = JsonConvert.DeserializeObject<DropSlotDef>(File.ReadAllText(custItem.Value.FilePath));
                Logger.M.WL(1, "id:" + def.Description.Id);
                Logger.M.WL(1, JsonConvert.SerializeObject(def, Formatting.Indented));
                def.Register();
              } catch (Exception e) {
                Logger.M.TWL(0, custItem.Key, true);
                Logger.M.WL(0, e.ToString(), true);
              }
            }
          }
        }
      } catch (Exception e) {
        Logger.M.TWL(0, e.ToString(), true);
      }
    }

    public static void Init(string directory, string settingsJSON) {
      BiggerDrops.ModDirectory = directory;
      Logger.BaseDirectory = directory;
      try {
        settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(Path.Combine(directory,"settings.json")));
        Logger.InitLog();
        Logger.M.TWL(0,"BiggerDrop log inited... " + directory + " version: " + Assembly.GetExecutingAssembly().GetName().Version, true);
      } catch (Exception e) {
        settings = new Settings();
        Logger.InitLog();
        Logger.M.TWL(0, "BiggerDrop log init exception "+e.ToString(), true);
      }
      try {
        var harmony = HarmonyInstance.Create("de.morphyum.BiggerDrops");
        harmony.PatchAll(Assembly.GetExecutingAssembly());
        ModDirectory = directory;
      } catch (Exception e) {
        Logger.M.TWL(0, e.ToString());
      }
    }
  }
}
