using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FCS_HomeSolutions.Mods.QuantumTeleporter.Mono;
using HarmonyLib;

namespace AdaptiveTeleportingCosts.Patches
{
    [HarmonyPatch(typeof(QTPowerManager))]
    internal class QTPowerManagerPatches
    {
        [HarmonyPatch(nameof(QTPowerManager.ModifyCharge))]
        public static bool Prefix(QTPowerManager __instance, float amount)
        {
            __instance.ConnectedRelay.ModifyPower(amount, out _);
            return false;//return is technically unnecessary, as the original method is empty anyway, but why not, right?
        }
    }
}
