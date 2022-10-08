using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoStorageTransfer.Monobehaviours;
using HarmonyLib;

namespace AutoStorageTransferCompatibility.Patches
{
    [HarmonyPatch(typeof(BaseBioReactorGeometry))]
    internal class BioReactorPatches
    {
        [HarmonyPatch(nameof(BaseBioReactorGeometry.Start))]
        public static void Postfix(BaseBioReactorGeometry __instance)
        {
            __instance.gameObject.AddComponent<StorageTransfer>().SetContainer(__instance.GetModule().container);
        }
    }
}
