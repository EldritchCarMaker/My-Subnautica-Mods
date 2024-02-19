using DroneBuddy.MonoBehaviours;
using HarmonyLib;

namespace DroneBuddy.Patches;

[HarmonyPatch(typeof(PlayerTool))]
internal class FixHoldingAnimation
{
    [HarmonyPatch(nameof(PlayerTool.animToolName), MethodType.Getter)]
    public static bool Prefix(PlayerTool __instance, ref string __result)
    {
        if (!__instance.TryGetComponent<Drone>(out var done))
            return true;
        __result = TechType.MapRoomCamera.ToString();
        return false;
    }
}
