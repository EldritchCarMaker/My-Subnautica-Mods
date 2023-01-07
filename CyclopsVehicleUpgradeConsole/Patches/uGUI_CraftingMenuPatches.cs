using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CyclopsVehicleUpgradeConsole.Monobehaviours;
using HarmonyLib;
#if SN2
using uGUI_CraftNode = uGUI_CraftingMenu.Node;
#endif

namespace CyclopsVehicleUpgradeConsole.Patches
{
    [HarmonyPatch(typeof(uGUI_CraftingMenu))]
    internal class uGUI_CraftingMenuPatches
    {
        [HarmonyPatch(nameof(uGUI_CraftingMenu.Action))]
        public static bool Prefix(uGUI_CraftNode sender, uGUI_CraftingMenu __instance)
        {
            if (sender == null) return true;

            if (uGUI.main?.craftingMenu?.client != null) return true;

            if(sender.icon == null) return true;

            if(!sender.icon.gameObject.TryGetComponent<MakeVehicleButton>(out var MVB)) return true;

#if SN1
            sender.Punch(0f, 0.5f);
#else
            __instance.Punch(sender.icon);
#endif
            FMODUWE.PlayOneShot(uGUI.main.craftingMenu.soundAccept, MainCamera.camera.transform.position, 1f);
            MVB.MakeVehicle();
            return false;
        }
    }
}
