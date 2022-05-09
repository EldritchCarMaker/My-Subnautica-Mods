using HarmonyLib;
using Logger = QModManager.Utility.Logger;
using System;
using System.Reflection;
using UnityEngine;

namespace CyclopsWindows
{
    [HarmonyPatch(typeof(SubRoot))]
    internal class SubRootPatches 
    {
        [HarmonyPatch(nameof(SubRoot.Awake))]
        [HarmonyPostfix]
        private static void Postfix(SubRoot __instance)
        {
            if (!__instance.isCyclops) return;
            if (__instance.GetComponent<SubRootMarker>()) return;
            //don't use ensure component because I do more stuff later and want to return first if component already exists
            __instance.gameObject.AddComponent<SubRootMarker>();

            CyclopsVehicleStorageTerminalManager cyclopsConsole = __instance.GetComponentInChildren<CyclopsVehicleStorageTerminalManager>();
            if (cyclopsConsole == null) return;

            GameObject cyclopsConsoleGUI = cyclopsConsole.gameObject.transform.Find("GUIScreen").gameObject;

            GameObject button = GameObject.Instantiate(cyclopsConsoleGUI.gameObject.transform.Find("Seamoth").Find("Modules").gameObject.GetComponent<CyclopsVehicleStorageTerminalButton>().gameObject, cyclopsConsoleGUI.gameObject.transform);

            button.transform.position += 12.265f * button.gameObject.transform.right;
            button.transform.position -= 6.3f * button.gameObject.transform.up;
            button.transform.position -= 0.127f * button.transform.forward;

            button.AddComponent<CyclopsWindows.MonoBehaviours.WindowButton>();

            GameObject.Destroy(button.GetComponent<CyclopsVehicleStorageTerminalButton>());
            button.name = "WindowButton";
        }
    }
}
