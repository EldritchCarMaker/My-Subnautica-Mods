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
            Transform transform1;
            Transform transform2;

            if (teleport1 is QuantumTeleporterController controller1)
                transform1 = controller1.transform;
            else if (teleport1 is QuantumTeleporterVehiclePadController pad1)
                transform1 = pad1.transform;
            else
                transform1 = Player.main.transform;

            if (teleport2 is QuantumTeleporterController controller2)
                transform2 = controller2.transform;
            else if (teleport2 is QuantumTeleporterVehiclePadController pad2)
                transform2 = pad2.transform;
            else
                transform2 = Player.main.transform;

            return GetTeleportCost(transform1, transform2);
        }
    }
}
