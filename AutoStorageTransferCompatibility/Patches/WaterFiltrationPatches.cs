using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoStorageTransfer.Monobehaviours;
using HarmonyLib;

namespace AutoStorageTransferCompatibility.Patches
{
    [HarmonyPatch(typeof(BaseFiltrationMachineGeometry))]
    internal class WaterFiltrationPatches
    {
        [HarmonyPatch(nameof(BaseFiltrationMachineGeometry.Start))]
        public static void Postfix(BaseFiltrationMachineGeometry __instance)
        {
            __instance.gameObject.AddComponent<StorageTransfer>().SetContainer(__instance.GetModule().storageContainer.container);
        }
    }
}
