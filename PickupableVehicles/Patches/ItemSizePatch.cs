﻿using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PickupableVehicles.Patches
{
#if SN
    [HarmonyPatch(typeof(CraftData))]
#else
    [HarmonyPatch(typeof(TechData))]
#endif
    internal class ItemSizePatch
    {
#if SN
        [HarmonyPatch(nameof(CraftData.GetItemSize))]
#else
        static List<TechType> seatruckTypes = new List<TechType>() { TechType.SeaTruck, TechType.SeaTruckAquariumModule, TechType.SeaTruckDockingModule, TechType.SeaTruckFabricatorModule, TechType.SeaTruckSleeperModule, TechType.SeaTruckStorageModule, TechType.SeaTruckTeleportationModule };
        [HarmonyPatch(nameof(TechData.GetItemSize))]
#endif
        public static void Postfix(TechType techType, ref Vector2int __result)
        {
            if (techType == TechType.Seamoth) __result = new Vector2int(QMod.config.seamothWidth, QMod.config.seamothHeight);
            else
            if (techType == TechType.Exosuit) __result = new Vector2int(QMod.config.prawnWidth, QMod.config.prawnHeight);
#if SN
            else
            if (techType == TechType.Cyclops) __result = new Vector2int(QMod.config.cyclopsWidth, QMod.config.cyclopsHeight);
            else
            if (techType == TechType.RocketBase) __result = new Vector2int(QMod.config.rocketWidth, QMod.config.rcketHeight);
#else
            else
            if (seatruckTypes.Contains(techType)) __result = new Vector2int(QMod.config.seatruckModuleWidth, QMod.config.seatruckModuleHeight);
            //if (techType.ToString().ToLower().Contains("seatruck")) __result = new Vector2int(QMod.config.seatruckModuleWidth, QMod.config.seatruckModuleHeight);
#endif
        }
    }
}
