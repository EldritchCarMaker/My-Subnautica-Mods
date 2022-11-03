using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using QuantumBase.Mono;

namespace QuantumBase.Patches
{
    [HarmonyPatch(typeof(StarshipDoor))]
    internal class StarshipDoorPatches
    {
        [HarmonyPatch(nameof(StarshipDoor.OnDoorToggle))]
        public static bool Prefix(StarshipDoor __instance)
        {
            if(__instance.TryGetComponent<QuantumExitDoor>(out _))
            {
                if (__instance.openSound)
                {
                    __instance.openSound.Play();
                }

                QuantumBase.main.SetPlayerOutBase();

                return false;
            }
            return true;
        }
    }
}
