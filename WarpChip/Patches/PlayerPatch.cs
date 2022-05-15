using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Logger = QModManager.Utility.Logger;

namespace WarpChip
{
    [HarmonyPatch(typeof(Player))]
    internal class PlayerPatch
    {
        [HarmonyPatch(nameof(Player.Start))]
        [HarmonyPostfix]
        public static void StartPostfix(Player __instance)
        {
            __instance.gameObject.EnsureComponent<WarpChipFunction>();
        }

        [HarmonyPatch(nameof(Player.Update))]
        [HarmonyPostfix]
        public static void UpdatePostfix(Player __instance)
        {
            if(Input.GetKeyDown(QMod.config.ControlKey) && Utility.EquipmentHasItem(WarpChipItem.thisTechType))
            {
                WarpChipFunction chip = __instance.gameObject.GetComponent<WarpChipFunction>();
                if(chip == null)
                {
                    Logger.Log(Logger.Level.Warn, "Player missing Warp Chip component");
                    chip = __instance.gameObject.AddComponent<WarpChipFunction>();
                }
                chip.TryTeleport();
            }
        }
    }
}
