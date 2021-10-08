using System;
using System.Collections.Generic;
using System.Reflection;
using BiggerDrops.Data;

namespace BiggerDrops.Features
{
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

    public static DropSlotDef GetDropSlotDef(string id)
    {
      if (dropSlotTypes.ContainsKey(id))
      {
        return dropSlotTypes[id];
      }

      return null;
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
}