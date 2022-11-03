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
        public static bool Prefix()
        {
            if (Player.main.currentSub is QuantumBase) return false;
            return true;
        }
    }
}
