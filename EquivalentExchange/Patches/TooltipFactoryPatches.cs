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
                ECMFontColor.Format($"ECM: {ExchangeMenu.GetCost(techType, useCreative: false)}", sb);
            }
        }
        public static bool ContainerIsValid(IItemsContainer IContainer)
        {
            return IContainer is ItemsContainer container && container.tr != null && container.tr.gameObject.GetComponentInParent<ItemResearchStation>();
        }
    }
    public static class ECMFontColor//thanks once again lee
    {
        private static Gradient _gradient = new Gradient()
        {
            colorKeys = new GradientColorKey[]
            {
                new GradientColorKey(new Color(1, 0, 0.75f), 0f), // color 0
                new GradientColorKey(new Color(1, 1, 1), 0.5f), // color 1
                new GradientColorKey(new Color(1, 0, 0.75f), 1f), // color 0
            }
        };

        private static float _glimmerTimeScale = 0.2f;

        public static void Format(string original, StringBuilder sb)
        {
            var chars = new char[original.Length];
            for (int i = 0; i < chars.Length; i++)
            {
                sb.Append($"<color=#{ColorUtility.ToHtmlStringRGB(EvaluateColorAtIndex(i, original.Length))}>{original[i]}</color>");
            }
        }

        private static Color EvaluateColorAtIndex(int index, int stringLength)
        {
            if (stringLength == 0) return Color.white;
            return _gradient.Evaluate(Mathf.Repeat((Time.realtimeSinceStartup - (float)index / stringLength) * _glimmerTimeScale, 1f));
        }
    }
}
