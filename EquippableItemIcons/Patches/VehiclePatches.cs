using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using EquippableItemIcons.API;

namespace EquippableItemIcons.Patches
{
    [HarmonyPatch(typeof(Vehicle))]
    internal class VehiclePatches
    {
        [HarmonyPatch(nameof(Vehicle.OnUpgradeModuleUse))]
        public static void Postfix(Vehicle __instance, TechType techType)
        {
            ErrorMessage.AddMessage("Module used");
            foreach (HudItemIcon icon in Registries.hudItemIcons)
            {
                if (icon.techType == techType)
                {
                    if (icon is ActivatedVehicleItem vehicleItem)
                        vehicleItem.HandleActivation();
                }
            }
        }
        [HarmonyPatch(nameof(Vehicle.ToggleSlot), new[] { typeof(int), typeof(bool) })]
        public static void Postfix(Vehicle __instance, int slotID, bool state)
        {
            ErrorMessage.AddMessage("Module toggled");
        }
    }
}
