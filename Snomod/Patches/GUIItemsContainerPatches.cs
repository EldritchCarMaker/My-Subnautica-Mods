using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Snomod.MonoBehaviours;

namespace Snomod.Patches
{
    [HarmonyPatch(typeof(uGUI_ItemsContainer))]
    internal class GUIItemsContainerPatches
    {
        public static MogusColorChanger.ColorType LastColorType = MogusColorChanger.ColorType.None;

        [HarmonyPatch(nameof(uGUI_ItemsContainer.OnAddItem))]
        public static void Prefix(InventoryItem item)
        {
            if (!item.item.TryGetComponent(out MogusColorChanger colorChanger))
            {
                LastColorType = MogusColorChanger.ColorType.None;
                return;
            }

            LastColorType = colorChanger.Color;
        }
    }
}
