using BattleTech.UI;
using Harmony;
using BiggerDrops.Features;

namespace BiggerDrops.Patches
{
    [HarmonyPatch(typeof(SGCmdCenterLanceConfigBG), "OnAddedToHierarchy")]
    public static class SGCmdCenterLanceConfigBG_OnAddedToHierarchy
    {
        static void Postfix(SGCmdCenterLanceConfigBG __instance)
        {
            if (CustomUnitsAPI.Detected() == false)
            {
                BiggerDrops.baysAlreadyAdded = 0;
                __instance.LC.UpdateSlotsCount(DropManager.DefaultMechSlots +
                                               DropManager.AdditionalMechSlots());
            }
        }
    }
}