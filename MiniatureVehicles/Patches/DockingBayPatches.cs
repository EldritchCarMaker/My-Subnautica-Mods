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
            var vehicle =
#if SN
                __instance.interpolatingVehicle;
#else
                __instance.interpolatingDockable;
#endif
            if (vehicle != null)
            {
                vehicle.transform.localScale = new UnityEngine.Vector3(1, 1, 1);
            }
            vehicle =
#if SN
                __instance.dockedVehicle;
#else
                __instance.dockedObject;
#endif

            if(vehicle != null)
            {
                vehicle.transform.localScale = new UnityEngine.Vector3(1, 1, 1);
            }
        }
    }
}
