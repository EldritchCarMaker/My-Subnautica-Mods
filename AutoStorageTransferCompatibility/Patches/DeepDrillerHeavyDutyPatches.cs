#if SN1
using AutoStorageTransferCompatibility.MonoBehaviours;
using FCS_ProductionSolutions.Mods.DeepDriller.HeavyDuty.Mono;
using FCS_ProductionSolutions.Mods.DeepDriller.LightDuty.Mono;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoStorageTransferCompatibility.Patches
{
    internal class DeepDrillerHeavyDutyPatches
    {
        public static void PatchDrill(Harmony harmony)
        {
            var postfix = AccessTools.Method(typeof(DeepDrillerHeavyDutyPatches), nameof(Postfix));
            harmony.Patch(AccessTools.Method(typeof(FCSDeepDrillerController), nameof(FCSDeepDrillerController.Start)), null, new HarmonyMethod(postfix));
        }
        public static void Postfix(FCSDeepDrillerController __instance)
        {
            __instance.gameObject.AddComponent<FCSDeepDrillerLightTransfer>().container = __instance.DeepDrillerContainer;
        }
    }
}
#endif