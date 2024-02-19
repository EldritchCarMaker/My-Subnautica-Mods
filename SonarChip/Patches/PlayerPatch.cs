using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilityStuffs;

namespace SonarChip
{
    [HarmonyPatch(typeof(Player))]
    internal class PlayerPatch
    {
        [HarmonyPatch(nameof(Player.Start))]
        [HarmonyPostfix]
        public static void StartPostfix(Player __instance)
        {
            __instance.gameObject.EnsureComponent<SonarChipMono>();
        }
    }
}