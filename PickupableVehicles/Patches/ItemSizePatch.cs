using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PickupableVehicles.Patches
{
    [HarmonyPatch(typeof(CraftData))]
    internal class ItemSizePatch
    {
        [HarmonyPatch(nameof(CraftData.GetItemSize))]
        public static void Postfix(TechType techType, ref Vector2int __result)
        {
            if (techType == TechType.Seamoth) __result = new Vector2int(QMod.config.seamothWidth, QMod.config.seamothHeight);
            else
            if (techType == TechType.Exosuit) __result = new Vector2int(QMod.config.prawnWidth, QMod.config.prawnHeight);
            else
            if (techType == TechType.Cyclops) __result = new Vector2int(QMod.config.cyclopsWidth, QMod.config.cyclopsHeight);
        }
    }
}
