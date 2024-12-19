using EquippableItemIcons.API;
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
using UnityEngine.SceneManagement;
using UWE;

namespace StasisSuit
{
    internal class StasisSuitMono : MonoBehaviour
    {
        public ActivatedEquippableItem hudItemIcon;
        public Player player;

        public void Awake()
        {
            player = GetComponent<Player>();

            var sprite = ImageUtils.LoadSpriteFromFile(Path.Combine(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets"), "StasisSuitIconRotate.png"));
            if(hudItemIcon == null)
            {
                hudItemIcon = new ActivatedEquippableItem("StasisSuitItem", ImageUtils.LoadSpriteFromFile(Path.Combine(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets"), "StasisSuitIconRotate.png")), StasisSuitItem.thisTechType);

                hudItemIcon.sprite = sprite;
                hudItemIcon.backgroundSprite = sprite;
                hudItemIcon.equipmentType = EquipmentType.Body;
                hudItemIcon.activateKey = QMod.config.StasisSuitKey;
                hudItemIcon.DeactivateSound = null;
                hudItemIcon.MaxIconFill = 60;
                hudItemIcon.ChargeRate = 5;

                Registries.RegisterHudItemIcon(hudItemIcon);

                hudItemIcon.container.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
            }

            hudItemIcon.Activate += Activate;
            hudItemIcon.Deactivate += Deactivate;
            hudItemIcon.OnUnEquip += Deactivate;
            hudItemIcon.CanActivate += canActivate;
        } 
        private void OnDestroy()
        {
            hudItemIcon.Activate -= Activate;
            hudItemIcon.Deactivate -= Deactivate;
            hudItemIcon.OnUnEquip -= Deactivate;
            hudItemIcon.CanActivate -= canActivate;
        }
        public bool canActivate()
        {
            return hudItemIcon.charge >= hudItemIcon.MaxCharge && hudItemIcon.CanActivateDefault();
        }
        public void Deactivate()
        {
            player.UnfreezeStats();
        }
        public void Activate()
        {
            player.FreezeStats();
        }
    }
}
