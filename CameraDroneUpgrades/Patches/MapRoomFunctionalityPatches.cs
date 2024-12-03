using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CameraDroneUpgrades.API;
using HarmonyLib;

namespace CameraDroneUpgrades.Patches
{
    [HarmonyPatch(typeof(MapRoomFunctionality))]
    internal class MapRoomFunctionalityPatches
    {
        [HarmonyPatch(nameof(MapRoomFunctionality.IsAllowedToAdd))]
        public static void Postfix(Pickupable pickupable, ref bool __result)
        {
            TechType techType = pickupable.GetTechType();

            foreach(CameraDroneUpgrade upgrade in Registrations.upgrades)
                if(upgrade.techType == techType) __result = true;
        }
    }
}
