using AutoStorageTransferCompatibility.MonoBehaviours;
using FCS_AlterraHub.Mods.AlterraHubDepot.Mono;
using FCS_ProductionSolutions.Mods.DeepDriller.HeavyDuty.Mono;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoStorageTransferCompatibility.Patches
{
    internal class AlterraHubDepotPatches
    {
        public static void PatchDepot(Harmony harmony)
        {
            var postfix = AccessTools.Method(typeof(AlterraHubDepotPatches), nameof(Postfix));
            harmony.Patch(AccessTools.Method(typeof(AlterraHubDepotController), nameof(AlterraHubDepotController.Start)), null, new HarmonyMethod(postfix));
        }
        public static void Postfix(AlterraHubDepotController __instance)
        {
            __instance.gameObject.AddComponent<FCSDepotTransfer>().container = __instance;
        }
    }
}
