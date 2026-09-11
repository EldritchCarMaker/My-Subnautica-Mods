using HarmonyLib;
using UnityEngine;

namespace MainMenuVideo;

[HarmonyPatch(typeof(ConsoleMainMenuNewsController), nameof(ConsoleMainMenuNewsController.DisplayNewsEntry))]
internal class ConsoleMainMenuNewsControllerPatch 
{
    [HarmonyPostfix]
    public static void Postfix(ConsoleMainMenuNewsController __instance)
    {
        var newsCanvasRenderer = __instance.GetComponentsInChildren<CanvasRenderer>(true);
        foreach (var renderer in newsCanvasRenderer)
        {
            renderer.SetAlpha(VideoSettingsManager.currentMenuOpacity);
        }
    }
}
