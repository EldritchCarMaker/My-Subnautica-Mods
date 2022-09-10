using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;

namespace CyclopsAntennaFix
{
    [HarmonyPatch(typeof(SubRoot))]
    internal class SubrootAwakePatch
    {
        [HarmonyPatch(nameof(SubRoot.Awake))]
        public static void Postfix(SubRoot __instance)
        {
            if (__instance.isCyclops) __instance.gameObject.AddComponent<AntennaPositionFixer>();
        }
    }
}
