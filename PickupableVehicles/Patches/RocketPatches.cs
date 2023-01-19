using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;

namespace PickupableVehicles.Patches
{
    [HarmonyPatch(typeof(Rocket))]
    internal class RocketPatches
    {
        [HarmonyPatch(nameof(Rocket.Start))]
        public static void Prefix(Rocket __instance)
        {
            if (__instance.TryGetComponent<Pickupable>(out var pick)) GameObject.DestroyImmediate(pick);
            if(__instance.name.Contains("Clone"))
                __instance.gameObject.EnsureComponent<ShiftPickuppableMono>().overrideTech = TechType.RocketBase;
        }
    }
}
