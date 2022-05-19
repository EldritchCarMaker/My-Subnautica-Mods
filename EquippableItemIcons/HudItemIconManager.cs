using EquippableItemIcons.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace EquippableItemIcons
{
    internal class HudItemIconManager : MonoBehaviour
    {
        public static HudItemIconManager main;
        public void Awake()
        {
            if(main != null)
            {
                Destroy(this);
                return;
            }
            main = this;
        }
        public void OnEquipmentChanged(string slot, InventoryItem item)
        {
            foreach(HudItemIcon icon in Registries.hudItemIcons)
            {
                icon.UpdateEquipped();
            }
            Registries.UpdatePositions();
        }
        public void Update()
        {
            foreach(HudItemIcon icon in Registries.hudItemIcons)
            {
                if(icon.AutomaticSetup) icon.Update();
            }
        }
    }
}
