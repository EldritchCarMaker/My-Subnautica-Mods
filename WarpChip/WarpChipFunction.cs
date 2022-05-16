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

namespace WarpChip
{
    internal class WarpChipFunction : MonoBehaviour
    {
        private Player player;
        private float timeNextTeleport = 0;

        private static readonly FMODAsset teleportSound = Utility.GetFmodAsset("event:/creature/warper/portal_open");

        private const float teleportDistanceOutside = 15;
        private const float teleportDistanceInside = 10;
        private const float teleportCooldown = 5;
        private const float teleportWallOffset = 1;//used so that you don't teleport partially inside of a wall, puts you slightly away from the wall

        public HudItemIcon itemIcon;
        public int FramesSinceCheck = 0;

        public void Awake()
        {
            itemIcon = new HudItemIcon("WarpChipIcon", ImageUtils.LoadSpriteFromFile(Path.Combine(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets"), "WarpChipIcon.png")), WarpChipItem.thisTechType);
            itemIcon.Deactivate += Deactivate;
            itemIcon.Activate += TryTeleport;
            itemIcon.activateKey = QMod.config.ControlKey;
            itemIcon.CanActivate += CanActivate;
            itemIcon.MaxCharge = 5;
            itemIcon.ChargeRate = 1;
            itemIcon.DrainRate = 5;

            Registries.RegisterHudItemIcon(itemIcon);

            player = GetComponent<Player>();
        }
        public void Update()
        {
            itemIcon.Update();
        }
        public void FixedUpdate()
        {
            if(FramesSinceCheck >= 10)
            {
                itemIcon.UpdateEquipped();
                FramesSinceCheck = 0;
            }
            FramesSinceCheck++;
        }
        public void TryTeleport()
        {
            if(Time.time >= timeNextTeleport && player != null && !player.isPiloting && player.mode == Player.Mode.Normal)
            {
                Teleport();
            }
        }

        public void Teleport()
        {
            float maxDistance = player.IsInside() ? teleportDistanceInside : teleportDistanceOutside;

            float distance = maxDistance;

            if (Targeting.GetTarget(player.gameObject, maxDistance, out var _, out float wallDistance))
            {
                distance = wallDistance - teleportWallOffset;
            }

            Transform aimingTransform = player.camRoot.GetAimingTransform();
            player.SetPosition(player.transform.position + aimingTransform.forward * distance);

            float cooldownTime = teleportCooldown / (maxDistance / distance);

            timeNextTeleport = Time.time + cooldownTime;

            TeleportScreenFXController fxController = MainCamera.camera.GetComponent<TeleportScreenFXController>();
            fxController.StartTeleport();
            CoroutineHost.StartCoroutine(TeleportFX());
            Utils.PlayFMODAsset(teleportSound, transform.position);
        }

        public static IEnumerator TeleportFX()
        {
            TeleportScreenFXController fxController = MainCamera.camera.GetComponent<TeleportScreenFXController>();
            yield return new WaitForSeconds(0.25f);
            fxController.StopTeleport();
        }
        public void Deactivate()
        {

        }
        public bool CanActivate()
        {
            return Time.time >= timeNextTeleport && itemIcon.charge == itemIcon.MaxCharge && player != null && !player.isPiloting && player.mode == Player.Mode.Normal;
        }
    }
}
