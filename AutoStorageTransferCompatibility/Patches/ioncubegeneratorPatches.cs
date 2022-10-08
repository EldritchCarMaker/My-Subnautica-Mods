using AutoStorageTransfer.Monobehaviours;
using HarmonyLib;
using IonCubeGenerator.Mono;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoStorageTransferCompatibility.Patches
{
    internal class ioncubegeneratorPatches
    {
        public static void PatchCubeGen(Harmony harmony)
        {
            var postfix = AccessTools.Method(typeof(ioncubegeneratorPatches), nameof(Postfix));
            harmony.Patch(AccessTools.Method(typeof(CubeGeneratorMono), nameof(CubeGeneratorMono.Awake)), null, new HarmonyMethod(postfix));
        }
        public static void Postfix(CubeGeneratorMono __instance)
        {
            __instance.gameObject.AddComponent<StorageTransfer>().SetContainer((__instance._cubeContainer as CubeGeneratorContainer)._cubeContainer);
        }
    }
}
