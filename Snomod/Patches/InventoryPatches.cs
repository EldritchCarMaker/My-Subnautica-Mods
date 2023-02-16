using HarmonyLib;
using Snomod.Prefabs;

namespace Snomod.Patches
{
    [HarmonyPatch(typeof(Inventory))]
    internal class InventoryPatches
    {
        [HarmonyPatch(nameof(Inventory.CanDropItemHere))]
        public static bool Prefix(ref bool __result, Pickupable item)
        {
            if(item.GetTechType() == Amogus.TT)
            {
                __result = true;
                return false;
            }
            return true;
        }
    }
}
