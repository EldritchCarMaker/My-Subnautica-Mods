using EldritchMoth.Mono;
using HarmonyLib;
using UnityEngine;

namespace EldritchMoth.Patches
{
    [HarmonyPatch(typeof(Vehicle))]
    internal class VehicleUpPatch
    {
        [HarmonyPatch(nameof(Vehicle.SlotNext))]
        public static bool Prefix(Vehicle __instance)
        {
            if(__instance.TryGetComponent(out EldritchMothMono mono))
            {
                mono.SpeedUp();
                return false;
            }
            return true;
        }
    }
    [HarmonyPatch(typeof(Vehicle))]
    internal class VehicleDownPatch
    {
        [HarmonyPatch(nameof(Vehicle.SlotPrevious))]
        public static bool Prefix(Vehicle __instance)
        {
            if(__instance.TryGetComponent(out EldritchMothMono mono))
            {
                mono.SpeedDown();
                return false;
            }
            return true;
        }
    }
    [HarmonyPatch(typeof(SeaMoth))]
    internal class VehicleNamePatch
    {
        [HarmonyPatch(nameof(SeaMoth.vehicleDefaultName))]
        [HarmonyPatch(MethodType.Getter)]
        public static void Postfix(Vehicle __instance, ref string __result)
        {
            if (__instance.GetComponent<EldritchMothMono>()) __result = Random.Range(0, 10) < 9 ? "Eld-Moth" : "L-Is-Gay";
        }
    }
}
