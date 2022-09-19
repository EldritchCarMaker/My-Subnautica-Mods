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
using WarpVehicleModule.Items;

namespace WarpVehicleModule
{
    internal class VehicleWarpFunction : MonoBehaviour
    {
        private static readonly FMODAsset teleportSound = Utility.GetFmodAsset("event:/creature/warper/portal_open");

        private float teleportDistance => Mathf.Clamp(QMod.config.DefaultWarpDistanceOutside, 0, MaxTeleportDistance);
        private float teleportCooldown => Mathf.Clamp(QMod.config.DefaultWarpCooldown, 0.1f, 20);

        private const float MaxTeleportDistance = 25;
        private const float teleportWallOffset = 1;//used so that you don't teleport partially inside of a wall, puts you slightly away from the wall

        public ActivatedVehicleItem itemIcon;

        public void Awake()
        {
            SetUpIcons();
        }
        public void SetUpIcons()
        {
            itemIcon = new ActivatedVehicleItem("VehicleWarpIcon", ImageUtils.LoadSpriteFromFile(Path.Combine(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets"), "VehicleWarpIconRotate.png")), WarpVehicleItem.thisTechType);
            itemIcon.Activate += TryTeleport;
            itemIcon.MaxCharge = 5;
            itemIcon.ChargeRate = itemIcon.MaxCharge / teleportCooldown;
            itemIcon.DrainRate = 0;
            itemIcon.ActivateSound = teleportSound;
            itemIcon.DeactivateSound = null;
            itemIcon.activationType = ActivatedVehicleItem.ActivationType.OnceOff;
            itemIcon.targetQuickslotType = typeof(Vehicle);
            Registries.RegisterHudItemIcon(itemIcon);
        }

        public void TryTeleport(Vehicle vehicle)
        {
            if(vehicle != null)
            {
                Teleport(vehicle);
            }
        }

        public void Teleport(Vehicle vehicle)
        {
            float distance = teleportDistance;

            if (Targeting.GetTarget(vehicle.gameObject, teleportDistance, out var _, out float wallDistance))
            {
                distance = wallDistance - teleportWallOffset;
            }

            Transform aimingTransform = vehicle.transform;
            vehicle.transform.position = vehicle.transform.position + aimingTransform.forward * distance;

            CoroutineHost.StartCoroutine(TeleportFX());

            if (itemIcon != null)
            {
                itemIcon.charge -= Mathf.Lerp(0f, itemIcon.MaxCharge, (100f / (teleportDistance / distance)) / 100f) / 2f;
            }
        }

        public static IEnumerator TeleportFX(float delay = 0.25f)
        {
            TeleportScreenFXController fxController = MainCamera.camera.GetComponent<TeleportScreenFXController>();
            fxController.StartTeleport();
            yield return new WaitForSeconds(delay);
            fxController.StopTeleport();
        }
    }
}
