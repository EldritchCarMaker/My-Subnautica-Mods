using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using CameraDroneUpgrades;
using UnityEngine;

namespace CameraDroneFlightUpgrade
{
    [HarmonyPatch]
    internal class MapRoomCameraPatch
    {
        [HarmonyPatch(typeof(MapRoomCamera), nameof(MapRoomCamera.FixedUpdate))]
        [HarmonyPrefix]
        public static bool FLYMYCHILD(MapRoomCamera __instance)
        {
            maproomdroneupgrades component = __instance.GetComponent<maproomdroneupgrades>();
            if (component == null || !component.equippedUpgrades.Contains(QMod.moduleTechType)) { return true; }

            __instance.rigidBody.AddForce(__instance.transform.rotation * (20f * __instance.wishDir), ForceMode.Acceleration);

            __instance.StabilizeRoll();

            return false;
        }
    }
}
