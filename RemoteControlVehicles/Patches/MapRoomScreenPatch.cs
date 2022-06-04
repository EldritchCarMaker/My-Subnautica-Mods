using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RemoteControlVehicles.Monobehaviours;

namespace RemoteControlVehicles.Patches
{
    [HarmonyPatch]
    internal class MapRoomScreenPatch
    {
        [HarmonyPatch(typeof(MapRoomScreen), nameof(MapRoomScreen.Start))]
        [HarmonyPostfix]
        public static void GetPrefab(MapRoomScreen __instance)
        {
            if (DroneControl.mapRoomScreenPrefab == null && __instance.cameraText != null) DroneControl.mapRoomScreenPrefab = __instance;
        }
        [HarmonyPatch(typeof(MapRoomFunctionality), nameof(MapRoomFunctionality.Start))]
        [HarmonyPostfix]
        public static void GetPrefab(MapRoomFunctionality __instance)
        {
            if (DroneControl.mapRoomScreenPrefab == null)
            {
                MapRoomScreen func = __instance.GetComponentInChildren<MapRoomScreen>();
                if(func != null && func.cameraText != null)
                    DroneControl.mapRoomScreenPrefab = func;
            }
        }
    }
}
