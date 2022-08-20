using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;

namespace CyclopsTorpedoes.Patches
{
    [HarmonyPatch(typeof(CyclopsDecoyScreenHUDManager))]
    internal class CyclopsDecoyScreenPatches
    {
        [HarmonyPatch(nameof(CyclopsDecoyScreenHUDManager.UpdateDecoyScreen))]
        public static void Postfix(CyclopsDecoyScreenHUDManager __instance)
        {
            int totalItemsEquipped = 0;
            foreach(KeyValuePair<TechType, int> pair in __instance.decoyManager.decoyLoadingTube.decoySlots.equippedCount)
            {
                totalItemsEquipped += pair.Value;
            }
            __instance.curCountText.text = totalItemsEquipped.ToString();
        }
    }
}
