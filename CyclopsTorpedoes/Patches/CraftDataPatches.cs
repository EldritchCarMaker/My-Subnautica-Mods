using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;

namespace CyclopsTorpedoes.Patches
{
    [HarmonyPatch(typeof(CraftData))]
    internal class CraftDataPatches
    {
        [HarmonyPatch(nameof(CraftData.GetEquipmentType))]
        public static void Postfix(TechType techType, ref EquipmentType __result)
        {
            if (CyclopsExternalCamsPatches.torpedoTypes == null)
            {
                CyclopsExternalCamsPatches.torpedoTypes = CraftData.GetPrefabForTechType(TechType.Seamoth).GetComponent<SeaMoth>().torpedoTypes;
            }

            List<TechType> torpedoTT = new List<TechType>();
            foreach(TorpedoType tt in CyclopsExternalCamsPatches.torpedoTypes) torpedoTT.Add(tt.techType);

            if (torpedoTT.Contains(techType)) __result = EquipmentType.DecoySlot;
        }
    }
}
