using FCS_HomeSolutions.Mods.QuantumTeleporter.Interface;
using FCS_HomeSolutions.Mods.QuantumTeleporter.Mono;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AdaptiveTeleportingCosts
{
    internal class TeleportUtils
    {
        public static float GetTeleportCost(Transform teleport1, Transform teleport2)
        {
            var distance = (teleport1.position - teleport2.position).magnitude;
            var cost = distance * QMod.config.distanceCostMultiplier;
            cost = Mathf.Clamp(cost, QMod.config.minimumTeleportCost, QMod.config.maximumTeleportCost);

            return cost;
        }
        public static float GetTeleportCost(IQuantumTeleporter teleport1, IQuantumTeleporter teleport2)
        {
            if (!(teleport1 is QuantumTeleporterController controller1) || !(teleport2 is QuantumTeleporterController controller2)) return 0;

            return GetTeleportCost(controller1.transform, controller2.transform);
        }
    }
}
