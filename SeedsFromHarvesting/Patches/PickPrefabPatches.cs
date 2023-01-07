using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeedsFromHarvesting.Patches
{
    [HarmonyPatch(typeof(PickPrefab))]
    internal class PickPrefabPatches
    {
        [HarmonyPatch(nameof(PickPrefab.OnHandClick)), HarmonyPostfix]
        public static void PostFix(PickPrefab __instance)
        {
            TechType techType = __instance.pickTech;
#if SN
            Vector2int size = CraftData.GetItemSize(techType);
#else
            Vector2int size = TechData.GetItemSize(techType);
#endif

            if (techType == TechType.Melon && Inventory.Get().HasRoomFor(size.x, size.y))
            {
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
