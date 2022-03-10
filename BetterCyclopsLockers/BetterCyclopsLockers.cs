using HarmonyLib;
using Logger = QModManager.Utility.Logger;
using System;
using System.Reflection;

namespace BetterCyclopsLockers
{
    [HarmonyPatch(typeof(CyclopsLocker), nameof(CyclopsLocker.Start))]
    internal class BetterCyclopsLockers
    {
        [HarmonyPostfix]
        private static void StartPostfix(CyclopsLocker __instance)
        {
            int Height = (int)QMod.Config.Height;
            int Width = (int)QMod.Config.Width;

            var storage = __instance.GetComponent<StorageContainer>(); // get the StorageContainer component that exists in the current locker object
            storage.Resize(Width, Height); // resize the locker. first argument is width, second is height.
            var temp = __instance.gameObject;
            temp.AddComponent<AutosortLockers.AutosortTarget>();
            temp.AddComponent<Constructable>();
        }
    }
    
    [HarmonyPatch(typeof(AutosortLockers.AutosortTarget), "Update")]
    internal class AutosortLockersUpdatePatch
    {
        [HarmonyPrefix]
        private static bool UpdatePrefix(AutosortLockers.AutosortTarget __instance)
        {
            var locker = __instance.GetComponent<CyclopsLocker>();
            if (locker != null) // if the current instance has a CyclopsLocker component, then it's a cyclops locker
            {
                return false; // prevent the code from executing
            }
            return true;
        }
    }
    [HarmonyPatch(typeof(AutosortLockers.AutosortTarget), "Save")]
    internal class AutosortLockersSavePatch
    {
        [HarmonyPrefix]
        private static bool SavePrefix(AutosortLockers.AutosortTarget __instance)
        {
            var locker = __instance.GetComponent<CyclopsLocker>();
            if (locker != null) // if the current instance has a CyclopsLocker component, then it's a cyclops locker
            {
                return false; // prevent the code from executing
            }
            return true;
        }
    }
}
