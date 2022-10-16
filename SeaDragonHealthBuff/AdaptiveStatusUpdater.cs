using FCS_AlterraHub.Helpers;
using FCS_AlterraHub.Mono;
using FCS_HomeSolutions.Mods.QuantumTeleporter.Mono;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AdaptiveTeleportingCosts
{
    internal class AdaptiveStatusUpdater : MonoBehaviour
    {
        public FcsDevice Unit { get; set; }

        private GameObject _icon;

        private QuantumTeleporterController _controller;

        public QuantumTeleporterController OriginalController;

        private void Start()
        {
            _icon = GameObjectHelpers.FindGameObject(gameObject, "ConnectionIcon", SearchOption.Full);
            _controller = (QuantumTeleporterController)Unit;
            OriginalController = GetComponentInParent<QuantumTeleporterController>();
        }

        private void Update()
        {
            if (Unit != null && _icon != null && _controller != null && OriginalController != null)
            {
                _icon.SetActive(_controller.PowerManager.PowerAvailable() >= TeleportUtils.GetTeleportCost(_controller, OriginalController));
            }
        }
    }
}
