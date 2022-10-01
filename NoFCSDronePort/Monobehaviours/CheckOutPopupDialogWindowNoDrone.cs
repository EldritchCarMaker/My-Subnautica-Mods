using FCS_AlterraHub.Mods.AlterraHubDepot.Mono;
using FCS_AlterraHub.Mods.FCSPDA.Mono.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace NoFCSDronePort.Monobehaviours
{
    internal class CheckOutPopupDialogWindowNoDrone : MonoBehaviour
    {
        public CheckOutPopupDialogWindow _normal;
        public CheckOutPopupDialogWindow Normal 
        { 
            get 
            { 
                if (_normal == null) 
                    _normal = GetComponent<CheckOutPopupDialogWindow>(); 
                return _normal; 
            } 
            set { _normal = value; } 
        }
        public AlterraHubDepotController SelectedDestination;
    }
}
