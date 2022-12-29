using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logger = QModManager.Utility.Logger;

namespace SeedsFromHarvesting
{
    [HarmonyPatch(typeof(GrownPlant))]
    public class GrownPlantPatches
    {
        [HarmonyPatch(nameof(GrownPlant.OnHandClick)), HarmonyPrefix]
        public static void Prefix(GrownPlant __instance, ref bool __state)
        {
            if (__instance.seed != null && !__instance.seed.isSeedling && __instance.seed.pickupable != null && Inventory.Get().HasRoomFor(__instance.seed.pickupable) && __instance.seed.currentPlanter != null)
            {
                __state = true;
                return;
            }
            __state = false; 
        }
        [HarmonyPatch(nameof(GrownPlant.OnHandClick)), HarmonyPostfix]
        public static void PostFix(GrownPlant __instance, bool __state)
        {
            if (__state)
            {
                TechType techType = CraftData.GetTechType(__instance.gameObject);
#if SN
                TechType harvestOutputData = CraftData.GetHarvestOutputData(techType);
#else
                TechType harvestOutputData = TechData.GetHarvestOutput(techType);
#endif
                CraftData.AddToInventory(harvestOutputData, 1, false, false);
            }
        }
    }
}