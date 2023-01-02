using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoStorageTransfer.Monobehaviours;
#if SN1
using CyclopsNuclearReactor;
using HarmonyLib;
using IonCubeGenerator.Mono;

namespace AutoStorageTransferCompatibility.Patches
{
    internal class CyclopsNukeReactorPatches
    {
        public static void PatchCyclopsNukeReactor(Harmony harmony)
        {
            var postfix = AccessTools.Method(typeof(CyclopsNukeReactorPatches), nameof(Postfix));
            harmony.Patch(AccessTools.Method(typeof(CyNukeReactorMono), nameof(CyNukeReactorMono.Awake)), null, new HarmonyMethod(postfix));
        }
        public static void Postfix(CyNukeReactorMono __instance)
        {
            __instance.gameObject.AddComponent<StorageTransfer>().SetContainer(__instance.container);
        }
    }
}
#endif