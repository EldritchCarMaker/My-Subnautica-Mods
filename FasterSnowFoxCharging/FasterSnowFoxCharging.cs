using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FasterSnowFoxCharging;
using HarmonyLib;
using UnityEngine;
using Logger = QModManager.Utility.Logger;

namespace RepairToolChanges_SN
{
    internal class RepairToolChanges
    {
        [HarmonyPatch(typeof(Hoverpad))]
        [HarmonyPatch("HealAndChargeBike")]
        internal class PatchHoverPad

        {
            [HarmonyPrefix]

            public static void Prefix(Hoverpad __instance)
            {
                if(QMod.Config.dockUsesPercentage)
                {
                    float chargeSpeed = __instance.dockedBike.energyMixin.capacity * (QMod.Config.dockChargeSpeed / 100);
                    float repairSpeed = __instance.dockedBike.liveMixin.maxHealth * (QMod.Config.dockRepairSpeed / 100);
                    __instance.healAmountPerTick = repairSpeed;
                    __instance.rechargeAmountPerTick = chargeSpeed;
                }
                else
                {
                    __instance.healAmountPerTick = QMod.Config.dockRepairSpeed;
                    __instance.rechargeAmountPerTick = QMod.Config.dockChargeSpeed;
                }
            }
        }
    }
}