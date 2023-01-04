using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;

namespace AntiUselessAirBladders.Patches
{
    [HarmonyPatch(typeof(PlayerTool))]
    internal class PlayerToolPatches
    {
        [HarmonyPatch(nameof(PlayerTool.OnAltDown))]
        public static void Postfix(PlayerTool __instance, ref bool __result)
        {
            if(__instance is AirBladder airBladder)
            {
                __result = true;
                Player.main.oxygenMgr.AddOxygen(5);
            }
        }
    }
}
