using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;

namespace MiniatureVehicles.Patches
{
    [HarmonyPatch(typeof(VehicleDockingBay))]
    internal class DockingBayPatches
    {
        [HarmonyPatch(nameof(VehicleDockingBay.OnTriggerEnter))]
        public static void Postfix(VehicleDockingBay __instance)
        {
            if(__instance.interpolatingVehicle != null)
            {
                __instance.interpolatingVehicle.transform.localScale = new UnityEngine.Vector3(1, 1, 1);
            }
            else if(__instance.dockedVehicle != null)
            {
                __instance.dockedVehicle.transform.localScale = new UnityEngine.Vector3(1, 1, 1);
            }
        }
    }
}
