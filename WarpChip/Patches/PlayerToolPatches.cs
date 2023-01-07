using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using WarpChip.Items;

namespace WarpChip.Patches
{
    [HarmonyPatch(typeof(PlayerTool))]
    internal class PlayerToolPatches
    {
        [HarmonyPatch(nameof(PlayerTool.animToolName))]
        [HarmonyPatch(MethodType.Getter)]
        public static void Postfix(PlayerTool __instance, ref string __result)
        {
            if (__instance.pickupable?.GetTechType() == TelePingBeacon.ItemTechType) __result = "beacon";
        }
    }
}
