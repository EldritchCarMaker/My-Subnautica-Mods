using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EquivalentExchange.Monobehaviours;
using HarmonyLib;
using UnityEngine;

namespace EquivalentExchange.Patches
{
    [HarmonyPatch(typeof(TooltipFactory))]
    internal class TooltipFactoryPatches
    {
        [HarmonyPatch(nameof(TooltipFactory.ItemCommons))]
        public static void Postfix(StringBuilder sb, TechType techType)
        {
            if (Inventory.main.usedStorage.Any((container) => ContainerIsValid(container)))
            {
                sb.AppendLine();
                sb.AppendLine();
                ECMFont.Format(techType, sb);
            }
        }
        public static bool ContainerIsValid(IItemsContainer IContainer)
        {
            return IContainer is ItemsContainer container && container.tr != null && (container.tr.gameObject.GetComponentInParent<ItemResearchStation>() || container.tr.gameObject.GetComponentInParent<AutomaticItemConverter>());
        }
    }
}
