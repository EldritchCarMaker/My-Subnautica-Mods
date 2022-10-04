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
    [HarmonyPatch(typeof(Equipment))]
    internal class EquipmentPatches
    {
        [HarmonyPatch(new Type[] {typeof(GameObject), typeof(Transform) })]
        [HarmonyPatch(MethodType.Constructor)]
        public static void Postfix(Equipment __instance)
        {
            __instance.owner.AddComponent<StorageTransfer>().SetContainer(__instance);
        }
    }
}
