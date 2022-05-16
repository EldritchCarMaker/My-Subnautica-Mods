using EquippableItemIcons.API;
using SMLHelper.V2.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UWE;
using Logger = QModManager.Utility.Logger;

namespace BurstFins
{
    internal class BurstFinsMono : MonoBehaviour
    {
        public HudItemIcon hudItemIcon = new HudItemIcon("BurstFinsIcon", SpriteManager.Get(TechType.UltraGlideFins), BurstFinsItem.thisTechType);
        public Player player;
        public int FixedUpdatesSinceCheck = 0;

        public void Awake()
        {
            player = GetComponent<Player>();

            hudItemIcon.name = "ShieldChipIcon"; 
            var sprite = SpriteManager.Get(TechType.UltraGlideFins);
            sprite.size = new Vector2(-sprite.size.x, sprite.size.y);
            
            hudItemIcon.sprite = sprite;
            hudItemIcon.backgroundSprite = sprite;
            hudItemIcon.CanActivate += CanActivate;
            hudItemIcon.IsIconActive += IsIconActive;
            hudItemIcon.activateKey = QMod.config.BurstFinsKey;
            hudItemIcon.techType = BurstFinsItem.thisTechType;
            hudItemIcon.equipmentType = EquipmentType.Foots;
            hudItemIcon.DrainRate = 20;
            
            Registries.RegisterHudItemIcon(hudItemIcon);
        }
        
        public void Update()
        {
            hudItemIcon.Update();
        }
        public void FixedUpdate()
        {
            if(FixedUpdatesSinceCheck > 20)
            {
                FixedUpdatesSinceCheck = 0;
                hudItemIcon.UpdateEquipped();
            }
            FixedUpdatesSinceCheck++;
        }
        public bool CanActivate()
        {
            return !player.isPiloting && !player.GetPDA().isOpen;
        }
        public bool IsIconActive()
        {
            return hudItemIcon.equipped;
        }
    }
}
