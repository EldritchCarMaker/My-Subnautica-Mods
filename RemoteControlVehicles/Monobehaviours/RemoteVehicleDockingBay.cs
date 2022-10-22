using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RemoteControlVehicles.Monobehaviours
{
    internal class RemoteVehicleDockingBay : MonoBehaviour
    {
        private const float TimeUntilVehicleAutoDock = 2;

        private DockableRemoteVehicle _currentVehicle;
        public DockableRemoteVehicle CurrentVehicle 
        { 
            get { return _currentVehicle; }
            private set
            {
                if (CurrentVehicle)
                    CurrentVehicle.SetDockingBay(null);

                _currentVehicle = value;

                if(CurrentVehicle)
                    CurrentVehicle.SetDockingBay(this);//I don't trust myself to remember to call the method every time, so do it automatically
            }
        }
        private DockableRemoteVehicle nearbyVehicle;
        private float timeNearbyVehicle;
        public void OnTriggerEnter(Collider c)
        {
            if (CurrentVehicle)
                return;

            var dockableVehicle = c.GetComponentInParent<DockableRemoteVehicle>();
            if (dockableVehicle)
            {
                nearbyVehicle = dockableVehicle;
                ErrorMessage.AddMessage($"Found vehicle {dockableVehicle}");
            }
        }
        public void Update()
        {
            var dockableVehicle = nearbyVehicle;
            if(dockableVehicle)
            {
                timeNearbyVehicle += Time.deltaTime;
                if(timeNearbyVehicle >= TimeUntilVehicleAutoDock || Input.GetKeyDown(QMod.config.dockControlKey))
                {
                    DockVehicle(dockableVehicle);
                }
            }
            else
            {
                timeNearbyVehicle = 0;
            }
        }
        public void DockVehicle(DockableRemoteVehicle vehicle)
        {
            nearbyVehicle = null;

            if (!vehicle)
            {
                UndockVehicle();
                return;
            }
            ErrorMessage.AddMessage("Docked vehicle");
            CurrentVehicle = vehicle;

            vehicle.transform.parent = transform;
            vehicle.gameObject.SetActive(false);
            vehicle.transform.localPosition = Vector3.zero;
            vehicle.transform.localRotation = Quaternion.identity;
        }
        public void UndockVehicle()
        {
            if (!CurrentVehicle) return;

            ErrorMessage.AddMessage("Undocked vehicle");

            CurrentVehicle.transform.parent = null;
            CurrentVehicle.transform.position = transform.position - (transform.up * 1);
            CurrentVehicle.gameObject.SetActive(true);
            CurrentVehicle = null;
        }
    }
}
