using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PickupableVehicles.Patches
{
#if BZ
    [HarmonyPatch(typeof(SeaTruckSegment))]
    internal class SeaTruckSegmentPatches
    {
        [HarmonyPatch(nameof(SeaTruckSegment.Start))]
        public static void Prefix(SeaTruckSegment __instance)
        {
            if(!QMod.config.worksWithSeatruck)
            {
                if(__instance.TryGetComponent<Pickupable>(out var pick)) GameObject.Destroy(pick);
                return;
            }

            __instance.gameObject.AddComponent<ShiftPickuppableMono>();
        }
    }
#endif
}
