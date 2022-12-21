using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FCS_HomeSolutions.Mods.QuantumTeleporter.Enumerators;
using FCS_HomeSolutions.Mods.QuantumTeleporter.Mono;
using FCSCommon.Utilities;
using HarmonyLib;
using QModManager.Utility;

namespace AdaptiveTeleportingCosts.Patches
{
    [HarmonyPatch(typeof(TeleportManager))]
    internal class TeleportManagerPatches
    {
        [HarmonyPatch(nameof(TeleportManager.TeleportPlayer), new Type[] { })]
        public static bool Prefix()
        {
            int cost = (int)TeleportUtils.GetTeleportCost(TeleportManager._currentTeleporter, TeleportManager._destinationTeleporter);

            TeleportManager._currentTeleporter.PowerManager.ModifyCharge(-cost);
            if(TeleportManager._tab != QTTeleportTypes.Intra) TeleportManager._destinationTeleporter.PowerManager.ModifyCharge(-cost);//don't want to drain power from base twice if both TPs are in same base
            TeleportManager.ToggleWarpScreen(true);
            TeleportManager._teleportComplete = false;
            TeleportManager._startTeleportCheck = true;
            Player.main.teleportingLoopSound.Play();
            TeleportManager.FreezeStats();
            if (TeleportManager._tab != QTTeleportTypes.Vehicle)
            {
                QModManager.Utility.Logger.Log(Logger.Level.Debug, "In Teleport Player");
                Player.main.playerController.inputEnabled = false;
                Inventory.main.quickSlots.SetIgnoreHotkeyInput(true);
                Player.main.GetPDA().SetIgnorePDAInput(true);
                Player.main.playerController.SetEnabled(false);
            }
            return false;
        }
    }
}
