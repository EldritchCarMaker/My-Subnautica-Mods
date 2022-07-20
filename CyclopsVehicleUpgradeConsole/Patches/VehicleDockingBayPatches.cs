using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using static CyclopsVehicleUpgradeConsole.VehicleConsoleCreation;

namespace CyclopsVehicleUpgradeConsole.Patches
{
    [HarmonyPatch(typeof(VehicleDockingBay))]
    internal class VehicleDockingBayPatches
    {
        [HarmonyPatch(nameof(VehicleDockingBay.DockVehicle))]
        [HarmonyPostfix]
        public static void SetTarget(VehicleDockingBay __instance, Vehicle vehicle)
        {
            SubRoot subRoot = __instance.GetComponentInParent<SubRoot>();

            if (!subRoot.isCyclops) return;

            SubNameInput subNameInput = subRoot.GetComponentInChildren<CyclopsVehicleStorageTerminalManager>().GetComponentInChildren<SubNameInput>(true);
            subNameInput.SetTarget(vehicle.subName);
            SetActive(__instance.gameObject);
        }
        [HarmonyPatch(nameof(VehicleDockingBay.OnUndockingStart))]
        [HarmonyPostfix]
        public static void RemoveTarget(VehicleDockingBay __instance)
        {
            SubRoot subRoot = __instance.GetComponentInParent<SubRoot>();

            if (!subRoot.isCyclops) return;

            SubNameInput subNameInput = subRoot.GetComponentInChildren<CyclopsVehicleStorageTerminalManager>().GetComponentInChildren<SubNameInput>(true);
            subNameInput.SetTarget(null);
            SetInActive(__instance.gameObject);
        }
    }
}
