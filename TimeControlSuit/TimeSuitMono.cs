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

namespace TimeControlSuit
{
    internal class TimeSuitMono : MonoBehaviour
    {
        public ActivatedEquippableItem hudItemIcon = new ActivatedEquippableItem("TimeSuitItem", ImageUtils.LoadSpriteFromFile(Path.Combine(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets"), "TimeSuitIconRotate.png")), TimeSuitItem.thisTechType);
        public Player player;
        public StasisSphere sphere;

        public void Awake()
        {
            player = GetComponent<Player>();
            SetUpSphere();

            var sprite = ImageUtils.LoadSpriteFromFile(Path.Combine(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets"), "TimeSuitIconRotate.png"));
            hudItemIcon.sprite = sprite;
            hudItemIcon.backgroundSprite = sprite;
            hudItemIcon.equipmentType = EquipmentType.Body;
            hudItemIcon.Activate += Activate;
            hudItemIcon.Deactivate += Deactivate;
            hudItemIcon.OnUnEquip += Deactivate;
            hudItemIcon.activateKey = QMod.config.TimeSuitKey;
            hudItemIcon.DeactivateSound = null;
            hudItemIcon.MaxIconFill = 60;
            hudItemIcon.CanActivate += CanActivate;

            hudItemIcon.MaxCharge = 10;
            hudItemIcon.ChargeRate = 1;
            hudItemIcon.DrainRate = 2;

            Registries.RegisterHudItemIcon(hudItemIcon);

        }
        public void SetUpSphere()
        {
            sphere = Instantiate(CraftData.GetPrefabForTechType(TechType.StasisRifle).GetComponent<StasisRifle>().effectSpherePrefab).GetComponent<StasisSphere>();
            sphere.gameObject.SetActive(true);
        }
        public void Deactivate()
        {
            sphere.CancelAll();
        }
        public void Activate()
        {
            sphere.transform.position = player.transform.position;
            sphere.radius = 300;
            sphere.time = 500f;
            sphere.fieldEnergy = 1;
            sphere.EnableField();
        }
        public bool CanActivate()
        {
            return hudItemIcon.CanActivateDefault() && hudItemIcon.charge >= hudItemIcon.MaxCharge;
        }
    }
}
