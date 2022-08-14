using HarmonyLib;
using System.Collections.Generic;

namespace CyclopsTorpedoes.Patches
{
    [HarmonyPatch(typeof(Equipment))]
    internal class EquipmentPatches
    {
        /*
        [HarmonyPatch(nameof(Equipment.AllowedToAdd))]
        public static void Postfix(Equipment __instance, ref bool __result, Pickupable pickupable)
        {
            if(pickupable.GetTechType() == TechType.GasTorpedo && __instance.owner != null && __instance.owner.GetComponent<CyclopsDecoyLoadingTube>())//is torpedo and cyclops tube
            {
                __result = true;
            }
        }
        [HarmonyPatch("IItemsContainer.AddItem"), new[] {typeof(InventoryItem)})]
        public static bool Prefix(Equipment __instance, ref bool __result, InventoryItem newItem)
        {
            if(__instance.owner != null && __instance.owner.GetComponent<CyclopsDecoyLoadingTube>() && newItem.item.GetTechType() == TechType.GasTorpedo)
            {
                string slot = null;

                foreach (KeyValuePair<EquipmentType, List<string>> keyValuePair in __instance.typeToSlots)
                {
                    Equipment.sSlots.AddRange(keyValuePair.Value);
                }
                for (int i = 0; i < Equipment.sSlots.Count; i++)
                {
                    string text = Equipment.sSlots[i];
                    InventoryItem inventoryItem;
                    __instance.equipment.TryGetValue(text, out inventoryItem);
                    if (inventoryItem == null || inventoryItem.ignoreForSorting)
                    {
                        slot = text;
                        break;
                    }
                }
                Equipment.sSlots.Clear();

                __result = __instance.AddItem(slot, newItem, false);
                return false;
            }
            return true;
        }
        */
    }
}
