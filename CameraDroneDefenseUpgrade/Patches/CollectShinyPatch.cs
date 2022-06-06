using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;

namespace CameraDroneDefenseUpgrade.Patches
{
    [HarmonyPatch(typeof(CollectShiny))]
    public class CollectShinyPatch
    {
        [HarmonyPatch(nameof(CollectShiny.TryPickupShinyTarget))]
        public static bool Prefix(CollectShiny __instance)
        {
            if (__instance.shinyTarget == null || !__instance.shinyTarget.activeInHierarchy) return true;
            
            if (__instance.shinyTarget.TryGetComponent(out CameraDroneUpgrades.maproomdroneupgrades asd) && asd.equippedUpgrades.Contains(Items.MapRoomCameraDefenseUpgrade.thisTechType))
            {
                if (!ZapFunctionality.Zap(asd.GetComponent<MapRoomCamera>(), __instance.gameObject)) return true;

                __instance.shinyTarget = null;
                __instance.targetPickedUp = false;
                __instance.timeNextFindShiny = Time.time + 10f;

                return false;
            }
            return true;
        }
    }
}
