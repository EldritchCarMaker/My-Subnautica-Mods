using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;

namespace RechargerChips
{
    [HarmonyPatch(typeof(Player))]
    internal class PatchToAddMono
    {
        [HarmonyPatch(nameof(Player.Start))]
        public static void Postfix(Player __instance)
        {
            __instance.gameObject.EnsureComponent<ChargerChipMono>();
        }
    }
}
