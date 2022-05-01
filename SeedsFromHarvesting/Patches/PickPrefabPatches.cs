using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logger = QModManager.Utility.Logger;

namespace SeedsFromHarvesting.Patches
{
    [HarmonyPatch(typeof(PickPrefab))]
    internal class PickPrefabPatches
    {
        [HarmonyPatch(nameof(PickPrefab.OnHandClick)), HarmonyPostfix]
        public static void PostFix(PickPrefab __instance)
        {
            TechType techType = __instance.pickTech;

            if(techType == TechType.Melon)
            {
                TechType harvestOutputData = CraftData.GetHarvestOutputData(techType);

                CraftData.AddToInventory(harvestOutputData, 1, false, false);
            }
        }
    }
}
