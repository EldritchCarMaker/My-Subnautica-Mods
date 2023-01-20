using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WarpChip.Monobehaviours
{
    public class TelePingVehicleInstance : TelePingInstance
    {
        private Vehicle _vehicle;
        public bool HasModule => _vehicle && /* && _vehicle.modules...has module*/true;
        public override bool TeleportAllowed => HasModule && _vehicle.IsPowered() && !_vehicle.docked;
        public override bool ShouldUseCinematicMode => false;
        private Color lastColor;
        public override void Teleport()
        {
            _vehicle.OnHandClick(Player.main.armsController.guiHand);
        }

        public override void Awake()
        {
            base.Awake();
            _vehicle = GetComponent<Vehicle>(); 
            _pingInstance.SetLabel("Teleping vecle");
            lastColor = ping._iconColor;
        }
        public override void Update()
        {
            if (HasModule)
                base.Update();
            else
                ping.SetColor(lastColor);//not good! Need to update lastColor more often! Preferably on module insertion, or even just patch the SetColor method
        }
    }
}
