using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WarpChip.Items;

namespace WarpChip.Monobehaviours
{
    public class TelePingVehicleInstance : TelePingInstance
    {
        private Vehicle _vehicle;
        public bool HasModule => _vehicle && _vehicle.modules.GetCount(TelePingVehicleModule.techType) > 0;
        public override bool TeleportAllowed => HasModule && _vehicle.IsPowered() && !_vehicle.docked;
        public override bool ShouldUseCinematicMode => false;
        private bool _hadModule = false;
        private Color _lastColor;
        public override void Teleport()
        {
            _vehicle.OnHandClick(Player.main.armsController.guiHand);
        }

        public override void Awake()
        {
            base.Awake();
            _vehicle = GetComponent<Vehicle>(); 
        }
        public override void Update()
        {
            var hasModule = HasModule;
            if(hasModule != _hadModule)
            {
                if (hasModule) 
                    _lastColor = ping._iconColor;
                else
                    ping.SetColor(_lastColor);
                _hadModule = hasModule;
            }
            if (hasModule)
                base.Update();
        }
    }
}
