using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;

namespace CyclopsVehicleUpgradeConsole.Patches
{
    [HarmonyPatch(typeof(VFXConstructing))]
    internal class VFXConstructingPatches
    {
        [HarmonyPatch(nameof(VFXConstructing.ApplySplashImpulse))]
        [HarmonyPrefix]
        public static bool PrefixApply(VFXConstructing __instance)
        {
            if(__instance.TryGetComponent(out Vehicle vehicle))
            {
                if (vehicle.docked) return false;
            }
            return true;
        }
        [HarmonyPatch(nameof(VFXConstructing.PlaySplashFX))]
        [HarmonyPrefix]
        public static bool PrefixPlayFX(VFXConstructing __instance)
        {
            if (__instance.TryGetComponent(out Vehicle vehicle))
            {
                if (vehicle.docked) return false;
            }
            return true;
        }
        [HarmonyPatch(nameof(VFXConstructing.PlaySplashSoundEffect))]
        [HarmonyPrefix]
        public static bool PrefixPlaySnd(VFXConstructing __instance)
        {
            if (__instance.TryGetComponent(out Vehicle vehicle))
            {
                if (vehicle.docked)
                {
                    FMODAsset asset = ScriptableObject.CreateInstance<FMODAsset>();
                    asset.path = "event:/sub/rocket/time_pod_close";
                    Utils.PlayFMODAsset(asset, __instance.transform);
                    return false;
                }
            }
            return true;
        }
    }
}
