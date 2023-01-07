using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UWE;
using static CyclopsVehicleUpgradeConsole.VehicleConsoleCreation;

namespace CyclopsVehicleUpgradeConsole.Patches
{
    [HarmonyPatch(typeof(SubRoot))]
    internal class SubrootPatches
    {
        [HarmonyPatch(nameof(SubRoot.Start))]
        [HarmonyPostfix]
        public static void Something(SubRoot __instance)
        {
            if (!__instance.isCyclops) return;

            if (!__instance.name.Contains("(Clone)")) return;//don't want to affect prefab cyclops, could cause duplicate objects/unset fields and properties

            VehicleDockingBay dockingBay = __instance.GetComponentInChildren<VehicleDockingBay>();
            CyclopsVehicleStorageTerminalManager cyclopsConsole = __instance.GetComponentInChildren<CyclopsVehicleStorageTerminalManager>();

            if (cyclopsConsole == null)
            {
                ErrorMessage.AddMessage("cyclops console null, returning. Unsure of what issues this could cause.");
                return;
            }

            //check if cyclops already has the console
            if (cyclopsConsole.GetComponentInChildren<SubNameInput>(true) != null)
            {
                return;
            }

            CoroutineHost.StartCoroutine(MakeUpgradeConsole(cyclopsConsole, dockingBay));
        }
    }
}
