using BattleTech;
using BattleTech.Save;
using Harmony;
using BiggerDrops.Data;
using BiggerDrops.Features;

namespace BiggerDrops.Patches
{
    [HarmonyPatch(typeof(SimGameState), "Rehydrate", typeof(GameInstanceSave))]
    class SimGameState_RehydratePatch
    {
        public static void Postfix(SimGameState __instance, GameInstanceSave gameInstanceSave)
        {
            if (BiggerDrops.settings.allowUpgrades)
            {
                BiggerDrops.settings.setCompanyStats(__instance.CompanyStats);
            }
        }
    }

    [HarmonyPatch(typeof(SimGameState), "InitCompanyStats")]
    class SimGameState_InitCompanyStatsPatch
    {
        public static void Postfix(SimGameState __instance)
        {
            if (BiggerDrops.settings.allowUpgrades)
            {
                BiggerDrops.settings.setCompanyStats(__instance.CompanyStats);
            }
        }
    }
    [HarmonyPatch(typeof(SimGameState), "AddArgoUpgrade")]
    class SimGameState_AddArgoUpgrade {
        public static void Postfix(SimGameState __instance) {
            if (BiggerDrops.settings.allowUpgrades) {
                BiggerDrops.settings.UpdateCULances();
            }
        }
    }
    [HarmonyPatch(typeof(SimGameState), "ApplyArgoUpgrades")]
    class SimGameState_ApplyArgoUpgrades {
        public static void Postfix(SimGameState __instance) {
            if (BiggerDrops.settings.allowUpgrades) {
                BiggerDrops.settings.UpdateCULances();
            }
        }
    }
}