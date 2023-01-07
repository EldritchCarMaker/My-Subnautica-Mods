using HarmonyLib;
using System;
using System.Reflection;
using UnityEngine;
using SMLHelper.V2.Utility;
using System.IO;
using UnityEngine.UI;
using UWE;
using System.Collections;

namespace CyclopsWindows
{
    [HarmonyPatch(typeof(SubRoot))]
    internal class SubRootPatches 
    {
        static readonly string AssetsFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");

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

            Component component = button.GetComponent<CyclopsVehicleStorageTerminalButton>();
            if(component != null) GameObject.Destroy(component);

            component = button.GetComponent<Image>();
            if (component != null) GameObject.Destroy(component);

            button.name = "WindowButton";

            Atlas.Sprite myAtlas = ImageUtils.LoadSpriteFromFile(Path.Combine(AssetsFolder, "window_toggle_full_alt.png"));
            var texture = myAtlas.texture;
            var sprite = UnityEngine.Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), Vector2.one * 0.5f);

            var child = button.gameObject.transform.GetChild(0);
            if (child != null)
            {
                var image = child.gameObject.GetComponent<UnityEngine.UI.Image>();

                if (image != null)
                    image.sprite = sprite;
            }
        }
    }
}
