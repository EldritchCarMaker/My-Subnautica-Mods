using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using WarpChip.Monobehaviours;

namespace WarpChip.Patches
{
    [HarmonyPatch(typeof(Vehicle))]
    internal class VehiclePatches
    {
        [HarmonyPatch(nameof(Vehicle.Awake))]
        public static void Prefix(Vehicle __instance)
        {
            __instance.gameObject.EnsureComponent<TelePingVehicleInstance>();
        }
    }
}
