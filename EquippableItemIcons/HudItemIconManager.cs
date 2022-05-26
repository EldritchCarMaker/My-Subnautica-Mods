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
        private static float FramesSinceCheck = 0;
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

            //commented out because for some god forsaken reason they don't seem to actually fucking do anything right now. why? Who the fuck knows why? I sure as hell don't 
            //too lazy and too pissed to keep debugging, just gonna rely on the periodic update checks to fix the positions
            //it's less than ideal, but I frankly don't give a shit anymore
            //Registries.UpdatePositions();
            //Registries.UpdatePositions();//for some weird fucking reason, updating only once wouldn't update properly
        }
        public void Update()
        {
            foreach(HudItemIcon icon in Registries.hudItemIcons)
            {
                if (icon.AutomaticSetup) 
                { 
                    if(icon is ActivatedEquippableItem equippableItem)
                        equippableItem.Update();

                    icon.Update();
                }
            }
            if(FramesSinceCheck >= 5)
            {
                Registries.UpdatePositions();
                FramesSinceCheck = 0;
            }
            FramesSinceCheck++;
        }
    }
}
