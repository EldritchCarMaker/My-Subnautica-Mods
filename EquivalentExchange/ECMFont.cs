using EquivalentExchange.Monobehaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static HandReticle;

namespace EquivalentExchange
{
    public static class ECMFont//thanks once again lee
    {
        private static Gradient _gradientGreen = new Gradient()
        {
            colorKeys = new GradientColorKey[]
            {
                new GradientColorKey(new Color(1, 0, 0.75f), 0f), // color 0
                new GradientColorKey(new Color(0, 1, 0), 0.5f), // color 1
                new GradientColorKey(new Color(1, 0, 0.75f), 1f), // color 0
            }
        }; 
        private static Gradient _gradientRed = new Gradient()
        {
            colorKeys = new GradientColorKey[]
            {
                new GradientColorKey(new Color(1, 0, 0.75f), 0f), // color 0
                new GradientColorKey(new Color(1, 0, 0), 0.5f), // color 1
                new GradientColorKey(new Color(1, 0, 0.75f), 1f), // color 0
            }
        };

        private static float _glimmerTimeScale = 0.2f;
        public static void Format(float cost, StringBuilder sb, bool includeCurrent = true)
        {
            float current = QMod.SaveData.ECMAvailable;
            if (includeCurrent)
            {
                FormatColor($"ECM: x{cost} ({current})", cost, current, sb);
            }
            else
            {
                FormatColor($"ECM: x{cost}", cost, current, sb);
            }
        }
        public static void Format(TechType techType, StringBuilder sb, bool includeCurrent = true)
        {
            Format(ExchangeMenu.GetCost(techType, useCreative: false), sb, includeCurrent);
        }
        public static void FormatColor(string original, float cost, float current, StringBuilder sb)
        {
            bool canAfford = current >= cost ||
#if SN
				                                !GameModeUtils.RequiresIngredients();
#else
                                                !GameModeManager.GetOption<bool>(GameOption.CraftingRequiresResources);
#endif


            if (!QMod.config.hotPinkECM)
            {
                if (canAfford)
                {
                    sb.Append("<color=#94DE00FF>");
                }
                else
                {
                    sb.Append("<color=#DF4026FF>");
                }
                sb.Append(original);
                sb.Append("</color>");
                return;
            }

            var chars = new char[original.Length];
            for (int i = 0; i < chars.Length; i++)
            {
                var color = EvaluateColorAtIndex(i, original.Length, canAfford);

                sb.Append($"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{original[i]}</color>");
            }
        }

        private static Color EvaluateColorAtIndex(int index, int stringLength, bool canAfford)
        {
            if (stringLength == 0) return Color.white;
            if(canAfford)
                return _gradientGreen.Evaluate(Mathf.Repeat((Time.realtimeSinceStartup - (float)index / stringLength) * _glimmerTimeScale, 1f));
            
            return _gradientRed.Evaluate(Mathf.Repeat((Time.realtimeSinceStartup - (float)index / stringLength) * _glimmerTimeScale, 1f));
        }
    }
}
