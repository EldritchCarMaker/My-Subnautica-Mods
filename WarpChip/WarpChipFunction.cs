using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
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

        public void Awake()
        {
            player = GetComponent<Player>();
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
    }
}
