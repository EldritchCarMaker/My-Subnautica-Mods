using HarmonyLib;
using System;
using System.Reflection;
using AutosortLockers;

namespace BetterCyclopsLockers
{
    [HarmonyPatch(typeof(CyclopsLocker), nameof(CyclopsLocker.Start))]
    internal class BetterCyclopsLockers
    {
        [HarmonyPostfix]
        private static void StartPostfix(CyclopsLocker __instance)
        {
            int Height = QMod.Config.Height;
            int Width = QMod.Config.Width;

            var storage = __instance.GetComponent<StorageContainer>(); // get the StorageContainer component that exists in the current locker object
            storage.Resize(Width, Height); // resize the locker. first argument is width, second is height.
            var temp = __instance.gameObject;
            temp.EnsureComponent<AutosortLockers.AutosortTarget>();
            Constructable constructable = temp.EnsureComponent<Constructable>();
            constructable.deconstructionAllowed = false;
        }
    }
    
    [HarmonyPatch(typeof(AutosortTarget), nameof(AutosortTarget.Update))]
    internal class AutosortLockersUpdatePatch
    {
        [HarmonyPrefix]
        private static bool UpdatePrefix(AutosortTarget __instance)
        {
            var locker = __instance.GetComponent<CyclopsLocker>();
            if (locker != null) // if the current instance has a CyclopsLocker component, then it's a cyclops locker
            {
                return false; // prevent the code from executing
            }
            return true;
        }
    }
    [HarmonyPatch(typeof(AutosortTarget), nameof(AutosortTarget.AddItem))]
    internal class StopCoroutinePatch
    {
        private static bool Prefix(AutosortTarget __instance, Pickupable item)
        {
            if (__instance.TryGetComponent(out CyclopsLocker locker))
            {
                __instance.container.container.AddItem(item);

                return false; 
            }
            return true;
        }
    }
#if SN1
    [HarmonyPatch(typeof(AutosortTarget), "Save")]
    internal class AutosortLockersSavePatch
    {
        [HarmonyPrefix]
        private static bool SavePrefix(AutosortTarget __instance)
        {
            var locker = __instance.GetComponent<CyclopsLocker>();
            if (locker != null) // if the current instance has a CyclopsLocker component, then it's a cyclops locker
            {
                return false; // prevent the code from executing
            }
            return true;
        }
    }
#endif
}
