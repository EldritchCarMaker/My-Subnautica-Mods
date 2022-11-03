using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;

namespace QuantumBase.Patches
{
    [HarmonyPatch(typeof(CrashedShipExploder))]
    internal class CrashedShipExploderPatches
    {
        [HarmonyPatch(nameof(CrashedShipExploder.UpdatePlayerCamShake))]
        public static bool Prefix()
        {
            if(Player.main.currentSub != null)
            {
                return false;
            }
            return true;
        }
    }
}
