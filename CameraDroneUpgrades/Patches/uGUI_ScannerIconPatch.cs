using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CameraDroneUpgrades.DroneScanning;
using HarmonyLib;

namespace CameraDroneUpgrades.Patches
{
    [HarmonyPatch(typeof(uGUI_ScannerIcon))]
    internal class uGUI_ScannerIconPatch
    {
        [HarmonyPatch(nameof(uGUI_ScannerIcon.Awake))]
        public static void Postfix(uGUI_ScannerIcon __instance)
        {
            ScanFunctionality.vanillaColor = __instance.icon.backgroundColorNormal;
        }
    }
}
