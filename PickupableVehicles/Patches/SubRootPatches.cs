using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;

namespace PickupableVehicles.Patches
{
#if SN
    [HarmonyPatch(typeof(SubRoot))]
    internal class SubRootPatches
    {
        [HarmonyPatch(nameof(SubRoot.Awake))]
        public static void Postfix(SubRoot __instance)
        {
            if (__instance.TryGetComponent<Pickupable>(out var pick)) GameObject.DestroyImmediate(pick);
            if(__instance.isCyclops && QMod.config.worksWithCyclops && __instance.name.Contains("Clone"))
            {
                __instance.gameObject.AddComponent<ShiftPickuppableMono>();
            }
        }
    }
#endif
}
