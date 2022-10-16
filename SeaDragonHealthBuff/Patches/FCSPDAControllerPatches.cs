using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FCS_AlterraHub.Mods.FCSPDA.Mono;
using HarmonyLib;

namespace AdaptiveTeleportingCosts.Patches
{
    [HarmonyPatch(typeof(FCSPDAController))]
    internal class FCSPDAControllerPatches
    {
        [HarmonyPatch("PlayAppropriateVoiceMessage")]
        public static bool Prefix()
        {
            return !QMod.config.stopPDAVoice;
        }
    }
}
