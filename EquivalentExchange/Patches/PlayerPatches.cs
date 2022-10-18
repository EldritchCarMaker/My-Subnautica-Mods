using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EquivalentExchange.Monobehaviours;
using HarmonyLib;

namespace EquivalentExchange.Patches
{
    [HarmonyPatch(typeof(Player))]
    internal class PlayerPatches
    {
        [HarmonyPatch(nameof(Player.Start))]
        public static void Postfix() => ExchangeMenu.EnsureCreated();
    }
}
