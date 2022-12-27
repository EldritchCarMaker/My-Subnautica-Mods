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
#if SN1
            SetUpSphere();
#else
            CoroutineHost.StartCoroutine(SetUpSphere());
#endif

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
            hudItemIcon.DrainRate = QMod.config.drainRate;

            Registries.RegisterHudItemIcon(hudItemIcon);

        }
#if SN1
        public void SetUpSphere()
        {
            sphere = Instantiate(CraftData.GetPrefabForTechType(TechType.StasisRifle).GetComponent<StasisRifle>().effectSpherePrefab).GetComponent<StasisSphere>();
            sphere.gameObject.SetActive(true);
        }
#else
        public IEnumerator SetUpSphere()
        {
            var task = CraftData.GetPrefabForTechTypeAsync(TechType.StasisRifle);
            yield return task;
            var riflePrefab = task.GetResult();

            var prefab = riflePrefab.GetComponent<StasisRifle>().effectSpherePrefab;
            sphere = Instantiate(prefab).GetComponent<StasisSphere>();
            sphere.gameObject.SetActive(true);
        }
#endif
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
