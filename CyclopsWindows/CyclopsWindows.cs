using HarmonyLib;
using Logger = QModManager.Utility.Logger;
using System;
using System.Reflection;
using UnityEngine;

namespace CyclopsWindows
{
    [HarmonyPatch(typeof(SubRoot), nameof(SubRoot.Awake))]
    internal class CyclopsWindows
    {
        [HarmonyPostfix]
        private static void StartPostfix(SubRoot __instance)
        {
            if (!__instance.isCyclops) return;
            if(__instance.gameObject.GetComponent<SubRootMarker>() != null) return;

            __instance.gameObject.EnsureComponent<SubRootMarker>();

            CyclopsLocker singleLocker = __instance.GetComponentInChildren<CyclopsLocker>();
            GameObject parent = singleLocker.gameObject.transform.parent.parent.parent.gameObject;
            CyclopsLocker[] lockers = parent.GetComponentsInChildren<CyclopsLocker>();
            int i = 0;
            foreach(CyclopsLocker locker in lockers)
            {
                i++;
                GameObject correctObject = locker.transform.parent.parent.gameObject;
                correctObject.transform.eulerAngles += new Vector3(0, 0, 180);
                correctObject.transform.position -= (i!=2 ? 2.65f : 2f) * locker.transform.right;
                correctObject.transform.position -= 1f * locker.transform.up;
            }
        }
    }
    public class SubRootMarker : MonoBehaviour
    {

    }
}
