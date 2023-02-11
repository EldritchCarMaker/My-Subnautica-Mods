using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmorSuit.Patches
{
    [HarmonyPatch(typeof(Player))]
    internal class PlayerPatch
    {
        [HarmonyPatch(nameof(Player.Awake))]
        public static void Postfix(Player __instance)
        {
            __instance.gameObject.EnsureComponent<ArmorSuitMono>();
        }
    }
}
