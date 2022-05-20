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
using UtilityStuffs;
using UWE;

namespace SonarChip
{
    internal class SonarChipMono : MonoBehaviour
    {
        private Player player;

        public HudItemIcon itemIcon;

        public void Awake()
        {
            itemIcon = new HudItemIcon("SonarChipIcon", ImageUtils.LoadSpriteFromFile(Path.Combine(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets"), "SonarChipIconRotate.png")), SonarChipItem.thisTechType);
            itemIcon.Activate += Activate;
            itemIcon.activateKey = QMod.config.ControlKey;
            itemIcon.MaxCharge = 5;
            itemIcon.ChargeRate = 1;
            itemIcon.RechargeDelay = 1;
            itemIcon.DrainRate = 1;
            itemIcon.ActivateSound = UtilityStuffs.Utility.GetFmodAsset("event:/sub/seamoth/sonar_loop");
            itemIcon.DeactivateSound = null;
            itemIcon.OnceOff = true;
            Registries.RegisterHudItemIcon(itemIcon);

            player = GetComponent<Player>();
        }
        public void Activate()
        {
            SNCameraRoot.main?.SonarPing();
        }
    }
}
