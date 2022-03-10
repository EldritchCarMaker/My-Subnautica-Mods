using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using System;
using Logger = QModManager.Utility.Logger;
using UnityEngine;

namespace SeaDragonHealthBuff
{
    [HarmonyPatch(typeof(Creature), "Start")]
    public class SeaDragon
    {
        [HarmonyPostfix]
        private static void startPostFix(Creature __instance)
        {
            if(__instance.GetType() == typeof(SeaDragon) && __instance.liveMixin.IsAlive())
            {
                __instance.liveMixin.data.maxHealth = 20000;
                __instance.liveMixin.ResetHealth();
            }
        }
    }
    [HarmonyPatch(typeof(LiveMixin), "TakeDamage")]
    public class Seadragon2
    {
        [HarmonyPrefix]
        private static void takedamagePreFix(LiveMixin __instance, float originalDamage)
        {
            if(__instance.GetType() == typeof(SeaDragon))
            {
                originalDamage = originalDamage / 10;
                Logger.Log(Logger.Level.Info, "yes", null, true);
            }
        }
    }
    [HarmonyPatch(typeof(Tweaks_Fixes.))]
}
