using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;

namespace BurstFins.Patches
{
    [HarmonyPatch(typeof(UnderwaterMotor))]
    internal class UnderWaterMotorPatches
    {
        [HarmonyPatch(nameof(UnderwaterMotor.AlterMaxSpeed))]
        public static void Postfix(ref float __result)
        {
            var mono = Player.main.GetComponent<BurstFinsMono>();
            if (mono && mono.hudItemIcon != null && mono.hudItemIcon.active)
            {
                __result = __result + 2.5f;
                __result = __result * 2f;
            }
            else if(mono && mono.hudItemIcon != null && mono.hudItemIcon.equipped)
            {
                __result = __result + 2.5f;
            }
        }
    }
}
