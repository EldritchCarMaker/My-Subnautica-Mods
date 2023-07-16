using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using LuckyBlock.MonoBehaviours;

namespace LuckyBlock.Patches
{
    [HarmonyPatch(typeof(Player))]
    internal class PlayerPatches
    {
        [HarmonyPatch(nameof(Player.Awake))]
        public static void Postfix(Player __instance)
        {
            __instance.gameObject.EnsureComponent<LuckyBlockSpawner>();
        }
    }
}
