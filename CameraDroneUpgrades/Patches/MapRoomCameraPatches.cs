using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;

namespace CameraDroneUpgrades.Patches
{
    [HarmonyPatch(typeof(MapRoomCamera))]
    internal class MapRoomCameraPatches
    {
        [HarmonyPatch(nameof(MapRoomCamera.ControlCamera))]
        public static void Postfix(MapRoomCamera __instance)
        {
            if(__instance.screen != null)
            {
                var upgrades = __instance.gameObject.EnsureComponent<maproomdroneupgrades>();
                upgrades.CountUpgrades(null);
            }
        }
    }
}
