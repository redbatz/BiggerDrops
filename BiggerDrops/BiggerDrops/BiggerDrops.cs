using BattleTech;
using Harmony;
using HBS.Collections;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using BiggerDrops.Data;
using BiggerDrops.Features;
using BiggerDrops.Patches;

namespace BiggerDrops {
  
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
