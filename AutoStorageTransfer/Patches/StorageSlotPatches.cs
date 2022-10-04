using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoStorageTransfer.Monobehaviours;
using HarmonyLib;
using UnityEngine;

namespace AutoStorageTransfer.Patches
{
    [HarmonyPatch(typeof(StorageSlot))]
    internal class StorageSlotPatches
    {
        [HarmonyPatch(new[] { typeof(Transform), typeof(string) })]
        [HarmonyPatch(MethodType.Constructor)]
        public static void Postfix(StorageSlot __instance)
        {
            __instance.root?.gameObject?.EnsureComponent<StorageTransfer>().SetContainer(__instance);
        }
    }
}
