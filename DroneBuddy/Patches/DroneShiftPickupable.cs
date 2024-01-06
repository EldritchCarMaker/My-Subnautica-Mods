using DroneBuddy.MonoBehaviours;
using HarmonyLib;

namespace DroneBuddy.Patches;

[HarmonyPatch(typeof(Pickupable))]
internal class DroneShiftPickupable 
{
    [HarmonyPatch(nameof(Pickupable.OnHandClick))]
    [HarmonyPrefix]
    public static bool ClickPrefix(Pickupable __instance, GUIHand hand)
    {
        if(!__instance.TryGetComponent<Drone>(out var drone))//not a drone
            return true;

        if(GameInput.GetButtonHeld(GameInput.Button.Sprint)) //holding shift
            return true;

        drone.OnHandClick(hand);
        return false;
    }
    [HarmonyPatch(nameof(Pickupable.OnHandHover))]
    [HarmonyPrefix]
    public static bool HoverPrefix(Pickupable __instance, GUIHand hand)
    {
        if (!__instance.TryGetComponent<Drone>(out var drone))//not a drone
            return true;

        if (GameInput.GetButtonHeld(GameInput.Button.Sprint)) //holding shift
            return true;

        drone.OnHandHover(hand);
        return false;
    }
}
