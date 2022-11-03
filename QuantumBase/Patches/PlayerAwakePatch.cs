using HarmonyLib;
using QuantumBase.Mono;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantumBase
{
    [HarmonyPatch(typeof(Player))]
    internal class PlayerAwakePatch
    {
        [HarmonyPatch(nameof(Player.Awake))]
        public static void Postfix()
        {
            Player.main.gameObject.EnsureComponent<PlayerInputTest>();

            QuantumBase.EnsureBase();
        }
    }
}
