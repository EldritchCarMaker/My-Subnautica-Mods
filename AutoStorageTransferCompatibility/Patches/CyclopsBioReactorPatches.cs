using AutoStorageTransfer.Monobehaviours;
#if SN1
using CyclopsBioReactor;
using HarmonyLib;
using IonCubeGenerator.Mono;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoStorageTransferCompatibility.Patches
{
    internal class CyclopsBioReactorPatches
    {
        public static void PatchCyclopsBioReactor(Harmony harmony)
        {
            var postfix = AccessTools.Method(typeof(CyclopsBioReactorPatches), nameof(Postfix));
            harmony.Patch(AccessTools.Method(typeof(CyBioReactorMono), nameof(CyBioReactorMono.Awake)), null, new HarmonyMethod(postfix));
        }
        public static void Postfix(CyBioReactorMono __instance)
        {
            __instance.gameObject.AddComponent<StorageTransfer>().SetContainer(__instance.container);
        }
    }
}
#endif