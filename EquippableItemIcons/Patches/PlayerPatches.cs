using EquippableItemIcons.API;
using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UWE;

namespace EquippableItemIcons.Patches
{
    [HarmonyPatch(typeof(Player))]
    internal class PlayerPatches
    {
        [HarmonyPatch(nameof(Player.Start))]
        public static void Postfix(Player __instance)
        {
            __instance.gameObject.AddComponent<HudItemIconManager>();
            if (Inventory.main == null || Inventory.main.equipment == null)
            {
                CoroutineHost.StartCoroutine(WaitForEquipment());
                return;
            }
            Inventory.main.equipment.onEquip += HudItemIconManager.main.OnEquipmentChanged;
            Inventory.main.equipment.onUnequip += HudItemIconManager.main.OnEquipmentChanged;
        }
        internal static IEnumerator WaitForEquipment()
        {
            yield return new WaitUntil(() => Inventory.main != null);
            yield return new WaitUntil(() => Inventory.main.equipment != null);
            Inventory.main.equipment.onEquip += HudItemIconManager.main.OnEquipmentChanged;
            Inventory.main.equipment.onUnequip += HudItemIconManager.main.OnEquipmentChanged;
            foreach(HudItemIcon icon in Registries.hudItemIcons)
            {
                if (icon.AutomaticSetup) icon.UpdateEquipped();
            }
        }
    }
}
