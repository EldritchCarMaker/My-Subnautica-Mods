using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Logger = QModManager.Utility.Logger;
using UtilityStuffs;

namespace WarpChip
{
    [HarmonyPatch(typeof(Player))]
    internal class PlayerPatch
    {
        [HarmonyPatch(nameof(Player.Start))]
        [HarmonyPostfix]
        public static void StartPostfix(Player __instance)
        {
            __instance.gameObject.EnsureComponent<WarpChipFunction>();
        }
    }
}