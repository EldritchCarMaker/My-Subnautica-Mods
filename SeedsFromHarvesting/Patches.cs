using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeedsFromHarvesting
{
    [HarmonyPatch]
    public class Patches
    {
        [HarmonyPatch(typeof(GrownPlant), nameof(GrownPlant.OnHandClick)), HarmonyPostfix]
        public static void PostFix(GrownPlant __instance)
        {
            if (__instance.seed != null && !__instance.seed.isSeedling && __instance.seed.pickupable != null && Inventory.Get().HasRoomFor(__instance.seed.pickupable) && __instance.seed.currentPlanter != null)
            {
                TechType techType = CraftData.GetTechType(__instance.gameObject);

                TechType harvestOutputData = CraftData.GetHarvestOutputData(techType);

                CraftData.AddToInventory(harvestOutputData, 1, false, false);
            }
        }
    }
}