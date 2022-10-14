using AutoStorageTransfer.Monobehaviours;
using AutoStorageTransferCompatibility.MonoBehaviours;
using CyclopsNuclearReactor;
using FCS_ProductionSolutions.Mods.DeepDriller.LightDuty.Mono;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoStorageTransferCompatibility.Patches
{
    internal class DeepDrillerLightDutyPatches
    {
        public static void PatchDrill(Harmony harmony)
        {
            var postfix = AccessTools.Method(typeof(DeepDrillerLightDutyPatches), nameof(Postfix));
            harmony.Patch(AccessTools.Method(typeof(DeepDrillerLightDutyController), nameof(DeepDrillerLightDutyController.Start)), null, new HarmonyMethod(postfix));
        }
        public static void Postfix(DeepDrillerLightDutyController __instance)
        {
            __instance.gameObject.AddComponent<FCSDeepDrillerLightTransfer>().container = __instance.DeepDrillerContainer;
        }
    }
}
