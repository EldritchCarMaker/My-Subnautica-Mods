﻿using EquippableItemIcons.API;
#if SN1
using SMLHelper.V2.Utility;
#else
using Nautilus.Utility;
#endif
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

        public static ActivatedEquippableItem itemIcon;

        public void Awake()
        {
            if (itemIcon == null)
            {
                itemIcon = new ActivatedEquippableItem("SonarChipIcon", ImageUtils.LoadSpriteFromFile(Path.Combine(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets"), "SonarChipIconRotate.png")), SonarChipItem.thisTechType);
                itemIcon.activateKey = QMod.config.ControlKey;
                itemIcon.MaxCharge = 5;
                itemIcon.ChargeRate = 1;
                itemIcon.RechargeDelay = 1;
                itemIcon.DrainRate = 1;
                itemIcon.ActivateSound = UtilityStuffs.Utility.GetFmodAsset("event:/sub/seamoth/sonar_loop");
                itemIcon.DeactivateSound = null;
                itemIcon.activationType = ActivatedEquippableItem.ActivationType.OnceOff;
                Registries.RegisterHudItemIcon(itemIcon);
            }
            itemIcon.Activate += Activate;

            player = GetComponent<Player>();
        }
        private void OnDestroy()
        {
            itemIcon.Activate -= Activate;
        }
        public void Activate()
        {
            SNCameraRoot.main?.SonarPing();
        }
    }
}
