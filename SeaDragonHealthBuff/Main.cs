using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using System;
using Logger = QModManager.Utility.Logger;
using UnityEngine;
using FCS_HomeSolutions.Mods.QuantumTeleporter.Mono;
using FCS_HomeSolutions.Mods.QuantumTeleporter.Enumerators;
using FCSCommon.Utilities;

namespace SeaDragonHealthBuff
{
    [HarmonyPatch]
    public static class QTPowerManagerPatch
    {
        //public static bool fromTakePower = false;
        [HarmonyPatch(typeof(FCS_HomeSolutions.Mods.QuantumTeleporter.Mono.QTPowerManager), nameof(FCS_HomeSolutions.Mods.QuantumTeleporter.Mono.QTPowerManager.TakePower))]
        [HarmonyPrefix]
        public static bool TakePowerPatch(QTPowerManager __instance, QTTeleportTypes type, ref bool __result)
        {
            QuantumTeleporterController teleporter1 = TeleportManager._currentTeleporter as QuantumTeleporterController;
            QuantumTeleporterController teleporter2 = TeleportManager._destinationTeleporter as QuantumTeleporterController;
            if (teleporter1 != null && teleporter2 != null)
            {
                Vector3.Distance(teleporter1.gameObject.transform.position, teleporter2.gameObject.transform.position);
                float distance = Vector3.Distance(teleporter1.gameObject.transform.position, teleporter2.gameObject.transform.position);
                //fromTakePower = true;
                if (__instance.HasEnoughPower(type))
                {
                    //fromTakePower = false;
                    __instance.ConnectedRelay.ConsumeEnergy(Mathf.Clamp(distance * 2, 1, 5000), out float num);
                    QuickLogger.Debug(string.Format("Consumed {0} amount of power for this operation", num), true);
                    __result = true;
                }
                else
                {
                    __result = false;
                }
            }

            return false;
        }
        /*Can't get to work
        [HarmonyPatch(typeof(FCS_HomeSolutions.Mods.QuantumTeleporter.Mono.QTPowerManager), nameof(FCS_HomeSolutions.Mods.QuantumTeleporter.Mono.QTPowerManager.TakePower))]
        [HarmonyPrefix]
        public static bool CheckPowerPatch(QTPowerManager __instance, ref bool __result)
        {
            if (fromTakePower)
            {
                QuantumTeleporterController teleporter1 = TeleportManager._currentTeleporter as QuantumTeleporterController;
                QuantumTeleporterController teleporter2 = TeleportManager._destinationTeleporter as QuantumTeleporterController;
                if (teleporter1 != null && teleporter2 != null)
                {
                    float distance = Vector3.Distance(teleporter1.gameObject.transform.position, teleporter2.gameObject.transform.position);
                    float powerCost = Mathf.Clamp(distance * QMod.config.distanceCostMultiplier, QMod.config.minimumTeleportCost, QMod.config.maximumTeleportCost);
                    __result = __instance.ConnectedRelay.GetPower() >= powerCost;
                }
            }
            else
            {
                __result = true;
            }
            return false;
        }*/
    }
}