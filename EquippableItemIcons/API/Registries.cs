using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UWE;

namespace EquippableItemIcons.API
{
    public static class Registries
    {
        private static List<HudItemIcon> hudItemIcons = new List<HudItemIcon>();
        private static List<HudItemIcon> activeIcons = new List<HudItemIcon>();
        private static bool CoroutineActive = false;

        public static void RegisterHudItemIcon(HudItemIcon icon)
        {
            if (hudItemIcons.Contains(icon))
            {
                QModManager.Utility.Logger.Log(QModManager.Utility.Logger.Level.Warn, $"Blocked duplicate icon {icon.name}");
                return;
            }
            hudItemIcons.Add(icon);
            UpdatePositions();
        }
        public static void UpdatePositions()
        {
            if (CoroutineActive) return;
            activeIcons.Clear();
            foreach(HudItemIcon icon in hudItemIcons)
            {
                if (icon.iconActive)
                {
                    activeIcons.Add(icon);
                    icon.container.SetActive(true);
                }
                else { icon.container.SetActive(false); }
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
            foreach (HudItemIcon icon in hudItemIcons)
            {
                icon.container.transform.localPosition = hudItemIcons.Count % 2 == 0 
                    ? quickSlots.GetPosition(0) - new Vector2(20 * (hudItemIcons.Count / 2), 0) 
                    : quickSlots.GetPosition(quickSlots.icons.Length) + new Vector2(20 * ((hudItemIcons.Count + 1 )/ 2), 0);
            }
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
