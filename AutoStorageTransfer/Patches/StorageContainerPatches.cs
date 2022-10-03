using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoStorageTransfer.Monobehaviours;
using HarmonyLib;

namespace AutoStorageTransfer.Patches
{
    [HarmonyPatch(typeof(StorageContainer))]
    internal class StorageContainerPatches
    {
        [HarmonyPatch(nameof(StorageContainer.Awake))]
        public static void Postfix(StorageContainer __instance)
        {
            __instance.gameObject.EnsureComponent<StorageTransfer>();
        }
    }
}
