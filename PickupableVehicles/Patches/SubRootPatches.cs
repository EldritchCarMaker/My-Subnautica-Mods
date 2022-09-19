using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;

namespace PickupableVehicles.Patches
{
    [HarmonyPatch(typeof(SubRoot))]
    internal class SubRootPatches
    {
        [HarmonyPatch(nameof(SubRoot.Awake))]
        public static void Postfix(SubRoot __instance)
        {
            if(__instance.isCyclops && QMod.config.worksWithCyclops)
            {
                __instance.gameObject.AddComponent<ShiftPickuppableMono>();
            }
        }
    }
}
