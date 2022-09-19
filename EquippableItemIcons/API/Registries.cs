using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UWE;
using Logger = QModManager.Utility.Logger;

namespace EquippableItemIcons.API
{
    public static class Registries
    {
        internal static List<HudItemIcon> hudItemIcons = new List<HudItemIcon>();
        internal static List<HudItemIcon> activeIcons = new List<HudItemIcon>();
        private static bool CoroutineActive = false;
        public static void RegisterHudItemIcon(HudItemIcon icon)
        {
            Logger.Log(Logger.Level.Info, $"Recieved HudItemIcon: {icon.name}"); 
            if (hudItemIcons.Contains(icon))
            {
                Logger.Log(Logger.Level.Warn, $"Blocked duplicate icon: {icon.name}");
                return;
            }
            hudItemIcons.Add(icon);
            icon.makeIcon();
            if(icon.container == null) Logger.Log(Logger.Level.Warn, $"{icon.name} has a null container. Unsure what problems this could cause exactly, but it could be an issue.");
            UpdatePositions();
        }
        public static void UpdatePositions()
        {
            if (CoroutineActive) return;

            activeIcons.Clear();
            foreach(HudItemIcon icon in hudItemIcons)
            {
                if(icon == null)
                {
                    hudItemIcons.Remove(icon);
                    continue;
                }

                if (icon.iconActive)
                {
                    activeIcons.Add(icon);
                }
                icon.container?.SetActive(false);
            }


            if(uGUI.main == null)
            {//safety check
                CoroutineActive = true;
                CoroutineHost.StartCoroutine(WaitForQuickSlots());
                return;
            }
            if (uGUI.main.quickSlots == null)
            {
                CoroutineActive = true;
                CoroutineHost.StartCoroutine(WaitForQuickSlots());
                return;
            }
            if (uGUI.main.quickSlots.icons == null)
            {
                CoroutineActive = true;
                CoroutineHost.StartCoroutine(WaitForQuickSlots());
                return;
            }

            uGUI_QuickSlots quickSlots = uGUI.main.quickSlots;

            var leftPos = quickSlots.GetPosition(0);
            var rightPos = quickSlots.GetPosition(quickSlots.icons.Length);
            int count = 0;
            var UseRightSide = true;
            List<HudItemIcon> iconsToRemove = new List<HudItemIcon>();
            foreach (HudItemIcon icon in activeIcons)
            {
                if (!icon.container)
                {
                    iconsToRemove.Add(icon);
                    continue;
                }
                icon.container.transform.localScale = new Vector3(QMod.config.iconSizeScale, QMod.config.iconSizeScale, QMod.config.iconSizeScale);

                icon.container.SetActive(quickSlots.target.GetType().IsSubclassOf(icon.targetQuickslotType) || quickSlots.target.GetType() == icon.targetQuickslotType);

                icon.container.transform.localPosition = !UseRightSide
                    ? new Vector2(leftPos.x - 80 - (80 * ((count - 1) / 2)), leftPos.y)
                    : new Vector2(rightPos.x + 10 + (80 * ((count) / 2)), rightPos.y);
                UseRightSide = !UseRightSide;
                count++;
            }
            iconsToRemove.ForEach(delegate (HudItemIcon icon)
            {
                activeIcons.Remove(icon);
            });
        }
        private static IEnumerator WaitForQuickSlots()
        {
            yield return new WaitUntil(() => uGUI.main != null);
            yield return new WaitUntil(() => uGUI.main.quickSlots != null);
            yield return new WaitUntil(() => uGUI.main.quickSlots.icons != null);
            CoroutineActive = false;
            UpdatePositions();
        }
    }
}
