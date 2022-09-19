using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarpVehicleModule.Items;

namespace WarpVehicleModule
{
    [HarmonyPatch(typeof(PDAScanner))]
    public static class PDAScannerUnlockPatch
    {
        [HarmonyPatch(nameof(PDAScanner.Unlock))]
        public static void Postfix(PDAScanner.EntryData entryData)
        {
            if (entryData.key == TechType.Warper)
            {
                if (!KnownTech.Contains(WarpVehicleItem.thisTechType))
                {
                    KnownTech.Add(WarpVehicleItem.thisTechType);
                    ErrorMessage.AddMessage("synthesized blueprint for warp chip");
                }
            }
        }
    }
}
