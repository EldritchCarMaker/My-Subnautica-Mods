using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using WarpChip.Monobehaviours;

namespace WarpChip.Patches
{
    [HarmonyPatch(typeof(Beacon))]
    internal class BeaconPatchs
    {
        [HarmonyPatch(nameof(Beacon.Throw))]
        public static void Postfix(Beacon __instance)
        {
            if (__instance.TryGetComponent<TelePingBeaconInstance>(out var ping))
#if SN
                ping.precursorOutOfWater = Player.main.precursorOutOfWater;
#else
                ping.precursorOutOfWater = Player.main.forceWalkMotorMode;
#endif
        }
    }
}
