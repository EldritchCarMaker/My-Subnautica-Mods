using HarmonyLib;
using UnityEngine;

namespace CyclopsWindows.Patches;

[HarmonyPatch(typeof(CyclopsCullManager))]
internal class CyclopsCullManagerPatches 
{
    [HarmonyPatch(nameof(CyclopsCullManager.Start))]
    public static void Prefix(CyclopsCullManager __instance)
    {
        //Find the canvas for the vehicle management console, and add it to the exclude list
        __instance.canvasesToExcludeFromCulling?.Add(__instance.transform.parent.GetComponentInChildren<CyclopsVehicleStorageTerminalManager>().GetComponentInChildren<Canvas>());
    }
}
