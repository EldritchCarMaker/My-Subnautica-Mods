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

namespace StasisSuit
{
    internal class StasisSuitMono : MonoBehaviour
    {
        public ActivatedEquippableItem hudItemIcon = new ActivatedEquippableItem("StasisSuitItem", ImageUtils.LoadSpriteFromFile(Path.Combine(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets"), "MiniSuitIconRotate.png")), StasisSuitItem.thisTechType);
        public Player player;

        public void Awake()
        {
            player = GetComponent<Player>();

            var sprite = ImageUtils.LoadSpriteFromFile(Path.Combine(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets"), "StasisSuitIconRotate.png"));
            hudItemIcon.sprite = sprite;
            hudItemIcon.backgroundSprite = sprite;
            hudItemIcon.equipmentType = EquipmentType.Body;
            hudItemIcon.Activate += Activate;
            hudItemIcon.Deactivate += Deactivate;
            hudItemIcon.OnUnEquip += Deactivate;
            hudItemIcon.activateKey = QMod.config.StasisSuitKey;
            hudItemIcon.CanActivate += canActivate;
            hudItemIcon.DeactivateSound = null;
            hudItemIcon.MaxIconFill = 60;

            Registries.RegisterHudItemIcon(hudItemIcon);

            hudItemIcon.container.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
        } 

        public bool canActivate()
        {
            return hudItemIcon.charge >= hudItemIcon.MaxCharge && hudItemIcon.CanActivateDefault();
        }
        public void Deactivate()
        {
            player.transform.localScale = Vector3.one;
        }
        public void Activate()
        {
            player.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        }
    }
}
