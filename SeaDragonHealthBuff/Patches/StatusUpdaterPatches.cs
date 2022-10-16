using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FCS_HomeSolutions.Mods.QuantumTeleporter.Mono;
using HarmonyLib;

namespace AdaptiveTeleportingCosts.Patches
{
    [HarmonyPatch(typeof(StatusUpdater))]
    internal class StatusUpdaterPatches
    {
        [HarmonyPatch(nameof(StatusUpdater.Update))]
        public static bool Prefix(StatusUpdater __instance)
        {
            /*
             * head back to later
             * doesn't seem to be big deal for now
             * may be good to deal with later though
            if (__instance.Unit != null && __instance._icon != null && __instance._controller != null)
            {
                __instance._icon.SetActive(__instance._controller.PowerManager.PowerAvailable() >= TeleportUtils.GetTeleportCost(__instance._controller, __instance._controller.));
            }
            return false;
            */
            return true;
        }
    }
}
