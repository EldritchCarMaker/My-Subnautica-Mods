using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EquivalentExchange.Monobehaviours;
using HarmonyLib;
using UnityEngine;
using UWE;

namespace EquivalentExchange.Patches
{
    [HarmonyPatch(typeof(uGUI_BuilderMenu))]
    internal class uGUI_BuilderMenuPatches
    {
        [HarmonyPatch(nameof(uGUI_BuilderMenu.Start))]
        public static void Postfix(uGUI_BuilderMenu __instance)
        {
            CoroutineHost.StartCoroutine(MakeExchangeMenu(__instance));
        }
        public static IEnumerator MakeExchangeMenu(uGUI_BuilderMenu origMenu)
        {
            ExchangeMenu exchangeMenu = null;

            yield return new WaitForEndOfFrame();
            while (exchangeMenu == null)
            {
                var cloneMenu = GameObject.Instantiate(origMenu.gameObject);
                GameObject.DestroyImmediate(cloneMenu.GetComponent<uGUI_BuilderMenu>());
                exchangeMenu = cloneMenu.AddComponent<ExchangeMenu>();
                yield return new WaitForSeconds(0.5f);
            }
        }

        [HarmonyPatch(nameof(uGUI_BuilderMenu.Awake))]
        public static bool Prefix(uGUI_BuilderMenu __instance)
        {
            if(__instance == null || __instance.TryGetComponent<ExchangeMenu>(out var menu))
            {
                return false;
            }
            return true;
        }
    }
}
