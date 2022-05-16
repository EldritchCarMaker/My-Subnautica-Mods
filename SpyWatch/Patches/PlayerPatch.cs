using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logger = QModManager.Utility.Logger;

namespace SpyWatch.Patches
{
    [HarmonyPatch(typeof(Player))]
    internal class PlayerPatch
    {
        [HarmonyPatch(nameof(Player.Awake))]
        public static void Postfix(Player __instance)
        {
            __instance.gameObject.EnsureComponent<SpyWatchMono>();
        }
        [HarmonyPatch(nameof(Player.CanBeAttacked))]
        public static bool Prefix(Player __instance, ref bool __result)
        {
            var watch = __instance.GetComponent<SpyWatchMono>();
            if (watch != null && watch.itemIcon.active)
            {
                __result = false;
                return false;
            }
            return true;
        }
    }
}
