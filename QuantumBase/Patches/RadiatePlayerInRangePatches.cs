using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;

namespace QuantumBase.Patches
{
    [HarmonyPatch(typeof(RadiatePlayerInRange))]
    internal class RadiatePlayerInRangePatches
    {
        [HarmonyPatch(nameof(RadiatePlayerInRange.Radiate))]
        [HarmonyPriority(Priority.High)]
        public static void Prefix(RadiatePlayerInRange __instance, out float __state)
        {
            __state = __instance.radiateRadius;

            if (Player.main.currentSub is QuantumBase) __instance.radiateRadius = 1;
        }

        [HarmonyPatch(nameof(RadiatePlayerInRange.Radiate))]
        public static void Postfix(RadiatePlayerInRange __instance, float __state)
        {
            __instance.radiateRadius = __state;
        }
    }
}
