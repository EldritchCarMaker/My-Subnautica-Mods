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
        //creates a new hud item icon
        public HudItemIcon hudItemIcon = new HudItemIcon("BurstFinsIcon", SpriteManager.Get(TechType.UltraGlideFins), BurstFinsItem.thisTechType);

        public void Awake()
        {
            var sprite = SpriteManager.Get(TechType.UltraGlideFins);
            sprite.size = new Vector2(-sprite.size.x, sprite.size.y);//tried to flip it upside down, didn't work too well there obviously
            
            hudItemIcon.backgroundSprite = sprite;//self explanatory

            hudItemIcon.activateKey = QMod.config.BurstFinsKey;//set the activate key to the config key

            hudItemIcon.equipmentType = EquipmentType.Foots;//fins use the foot slot, set it here

            hudItemIcon.DrainRate = 20;//want this to be short lasting, only lasts for 5 seconds here. Max is 100, drain is 20 per second, 5 seconds of use.
            
            Registries.RegisterHudItemIcon(hudItemIcon);//where all the magic happens
        }
    }
}
