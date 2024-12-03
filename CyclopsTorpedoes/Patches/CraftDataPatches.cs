using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;

namespace CyclopsTorpedoes.Patches
{
    [HarmonyPatch(typeof(CraftData))]
    internal class CraftDataPatches
    {
        [HarmonyPatch(nameof(CraftData.GetEquipmentType))]
        public static void Postfix(TechType techType, ref EquipmentType __result)
        {
            List<TechType> torpedoTT = new List<TechType>();
            foreach(TorpedoType tt in CyclopsExternalCamsPatches.torpedoTypes) torpedoTT.Add(tt.techType);

            if (torpedoTT.Contains(techType)) __result = EquipmentType.DecoySlot;
        }
    }
}
