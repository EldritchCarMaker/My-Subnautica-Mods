using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvincibleDockedVehicles
{
    [HarmonyPatch(typeof(Vehicle))]
    internal class VehiclePatches
    {
        [HarmonyPatch(nameof(Vehicle.OnDockedChanged))]
        public static void Postfix(Vehicle __instance, bool docked)
        {
            if(docked)
            {
                __instance.liveMixin.shielded = true;
            }
            else
            {
                __instance.liveMixin.shielded = false;
            }
        }
        [HarmonyPatch(nameof(Vehicle.Awake))]
        public static void Postfix(Vehicle __instance)
        {
            if (__instance._docked)
            {
                __instance.liveMixin.shielded = true;
            }
            else
            {
                __instance.liveMixin.shielded = false;
            }
        }

    }
}
