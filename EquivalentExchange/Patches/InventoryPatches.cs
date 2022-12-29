using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EquivalentExchange.Monobehaviours;
using HarmonyLib;

namespace EquivalentExchange.Patches
{
    [HarmonyPatch(typeof(Inventory))]
    internal class InventoryPatches
    {
        [HarmonyPatch(nameof(Inventory.ExecuteItemAction), new[] { typeof(ItemAction), typeof(InventoryItem)})]//stops player from being able to eat things that they can't remove from the container
        public static bool Prefix(InventoryItem item, ItemAction action)
        {
            if (action != ItemAction.Use && action != ItemAction.Eat) return true;
            if (item == null) return true;
            if (item.container == null) return true;

            if (!item.container.AllowedToRemove(item.item, false)) return false;
            if(item.container is ItemsContainer container && container.tr.GetComponentInParent<AutomaticItemConverter>()) return false;

            return true;
        }
    }
}
