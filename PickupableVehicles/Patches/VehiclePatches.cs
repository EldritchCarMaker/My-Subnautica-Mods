using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PickupableVehicles
{
    [HarmonyPatch(typeof(Vehicle))]
    internal class VehiclePatches
    {
        [HarmonyPatch(nameof(Vehicle.Awake))]
        public static void Postfix(Vehicle __instance)
        {
            __instance.gameObject.EnsureComponent<Pickupable>();
        }
        [HarmonyPatch(nameof(Vehicle.OnHandClick))]
        [HarmonyPrefix]
        public static bool ClickPrefix(Vehicle __instance)
        {
            if(GameInput.GetButtonHeld(GameInput.Button.Sprint))
            {
                if (!QMod.config.worksWithPrawn && __instance is Exosuit) return true;

                if (QMod.config.needsModule && __instance.modules.GetCount(PickupableVehicleModule.thisTechType) <= 0) return true;

                Pickupable pickup = __instance.gameObject.GetComponent<Pickupable>();
                if (pickup != null) pickup.OnHandClick(Player.main.armsController.guiHand);
                return false;
            }
            return true;
        }
        [HarmonyPatch(nameof(Vehicle.OnHandHover))]
        [HarmonyPrefix]
        public static bool HoverPrefix(Vehicle __instance)
        {
            if (GameInput.GetButtonHeld(GameInput.Button.Sprint))
            {
                if (!QMod.config.worksWithPrawn && __instance is Exosuit) return true;

                if (QMod.config.needsModule && __instance.modules.GetCount(PickupableVehicleModule.thisTechType) <= 0) return true;

                Pickupable pickup = __instance.gameObject.GetComponent<Pickupable>();
                if (pickup != null) pickup.OnHandHover(Player.main.armsController.guiHand);
                return false;
            }
            return true;
        }
    }
}
