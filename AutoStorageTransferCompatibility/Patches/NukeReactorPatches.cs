using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoStorageTransfer.Monobehaviours;

namespace AutoStorageTransferCompatibility.Patches
{
    [HarmonyPatch(typeof(BaseNuclearReactorGeometry))]
    internal class NukeReactorPatches
    {
        [HarmonyPatch(nameof(BaseNuclearReactorGeometry.Start))]
        public static void Postfix(BaseNuclearReactorGeometry __instance)
        {
            __instance.gameObject.AddComponent<StorageTransfer>().SetContainer(__instance.GetModule().equipment);
        }
    }
}
