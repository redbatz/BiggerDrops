using BattleTech;
using BattleTech.Save;
using BattleTech.Save.SaveGameStructure;
using Harmony;
using System;
using BiggerDrops.Data;
using BiggerDrops.Features;

namespace BiggerDrops.Patches
{
    [HarmonyPatch(typeof(GameInstanceSave), MethodType.Constructor)]
    [HarmonyPatch(new Type[] { typeof(GameInstance), typeof(SaveReason) })]
    public static class GameInstanceSave_Constructor_Patch {
        static void Postfix(GameInstanceSave __instance) {
            Helper.SaveState(__instance.InstanceGUID, __instance.SaveTime);
        }
    }

    [HarmonyPatch(typeof(GameInstance), "Load")]
    public static class GameInstance_Load_Patch {
        static void Prefix(GameInstanceSave save) {
            Helper.LoadState(save.InstanceGUID, save.SaveTime);
        }
    }
}