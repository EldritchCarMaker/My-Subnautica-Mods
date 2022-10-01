using FCS_AlterraHub.Mods.AlterraHubDepot.Mono;
using FCS_AlterraHub.Mods.AlterraHubFabricatorBuilding.Mono.DroneSystem;
using FCS_AlterraHub.Mods.FCSPDA.Mono.Dialogs;
using FCSCommon.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace NoFCSDronePort.Monobehaviours
{
    internal class AlterraHubDepotItemControllerNoDrone : MonoBehaviour
    {
        public AlterraHubDepotController Destination { get; set; }
        private AlterraHubDepotItemController _normal;
        public AlterraHubDepotItemController Normal 
        { 
            get 
            { 
                if (_normal == null) 
                    _normal = GetComponent<AlterraHubDepotItemController>(); 
                return _normal; 
            } 
            set { _normal = value; }
        }
        internal bool Initialize(AlterraHubDepotItemController normal, AlterraHubDepotController depot, ToggleGroup toggleGroup, Transform list)
        {
            bool result;
            try
            {
                if (((depot != null) ? depot.Manager : null) == null || toggleGroup == null || list == null)
                {
                    result = false;
                }
                else
                {
                    Normal = normal;
                    Destination = depot;
                    gameObject.FindChild("ItemName").GetComponent<Text>().text = "Name: " + depot.Manager.GetBaseName() + "\nStatus: " + depot.GetStatus();
                    Normal._toggleGroup = toggleGroup;
                    Normal._toggle = base.gameObject.GetComponentInChildren<Toggle>();
                    Normal._toggle.group = toggleGroup;
                    if (depot.IsFull())
                    {
                        Normal._toggle.enabled = false;
                        Normal._toggle.isOn = false;
                    }
                    gameObject.transform.localScale = Vector3.one;
                    gameObject.transform.SetParent(list, false);
                    result = true;
                }
            }
            catch (Exception ex)
            {
                QuickLogger.Error(ex.Message, false);
                QuickLogger.Error(ex.StackTrace, false);
                result = false;
            }
            return result;
        }
    }
}
