using EquippableItemIcons.API;
using SMLHelper.V2.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Logger = QModManager.Utility.Logger;

namespace ShieldChip
{
    internal class ShieldChipMono : MonoBehaviour
    {
        public HudItemIcon hudItemIcon = new HudItemIcon("ShieldChipIcon", ImageUtils.LoadSpriteFromFile(Path.Combine(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets"), "ShieldChipIconRotate.png")), ShieldChipItem.thisTechType);
        public Player player;
        public int FixedUpdatesSinceCheck = 0;

        public void Awake()
        {
            player = GetComponent<Player>();

            hudItemIcon.name = "ShieldChipIcon";
            var sprite = ImageUtils.LoadSpriteFromFile(Path.Combine(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets"), "ShieldChipIconRotate.png"));
            hudItemIcon.sprite = sprite;
            hudItemIcon.backgroundSprite = sprite;
            hudItemIcon.Activate += Activate;
            hudItemIcon.Deactivate += Deactivate;
            hudItemIcon.CanActivate += CanActivate;
            hudItemIcon.IsIconActive += IsIconActive;
            hudItemIcon.activateKey = QMod.config.ShieldChipKey;
            hudItemIcon.techType = ShieldChipItem.thisTechType;

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
        public void Deactivate()
        {
            Logger.Log(Logger.Level.Info, "Fuck yea you pressed the button", null, true);
            player.liveMixin.shielded = false;
        }
        public void Activate()
        {
            Logger.Log(Logger.Level.Info, "Fuck yea you pressed the button", null, true);
            player.liveMixin.shielded = true;
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
