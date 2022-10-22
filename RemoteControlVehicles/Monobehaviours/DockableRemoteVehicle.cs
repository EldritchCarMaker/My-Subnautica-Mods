using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RemoteControlVehicles.Monobehaviours
{
    internal class DockableRemoteVehicle : MonoBehaviour
    {
        public RemoteVehicleDockingBay CurrentBay { get; private set; }
        public void SetDockingBay(RemoteVehicleDockingBay bay)
        {
            if (!bay)
            {
                ClearDockingBay();
                return;
            }
            CurrentBay = bay;
        }
        public void ClearDockingBay()
        {
            CurrentBay = null;
        }
    }
}
