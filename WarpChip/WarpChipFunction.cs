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

        private static readonly FMODAsset teleportSound = Utility.GetFmodAsset("event:/creature/warper/portal_open");

        private const float teleportDistanceOutside = 15;
        private const float teleportDistanceInside = 10;
        private const float teleportCooldown = 5;
        private const float teleportWallOffset = 1;//used so that you don't teleport partially inside of a wall, puts you slightly away from the wall

        public ActivatedEquippableItem itemIcon;
        public ChargableEquippableItem chargingIcon;
        //public bool UpgradedItemEquipped = false;
        public int FramesSinceCheck = 0;
        bool justTeleportedToBase = false;

        public void Awake()
        {
            SetUpIcons();

            player = GetComponent<Player>();
        }
        public void SetUpIcons()
        {
            itemIcon = new ActivatedEquippableItem("WarpChipIcon", ImageUtils.LoadSpriteFromFile(Path.Combine(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets"), "WarpChipIconRotate.png")), WarpChipItem.thisTechType);
            itemIcon.DetailedActivate += TryTeleport;
            itemIcon.activateKey = QMod.config.ControlKey;
            itemIcon.MaxCharge = 5;
            itemIcon.ChargeRate = 1;
            itemIcon.DrainRate = 0;
            itemIcon.ActivateSound = teleportSound;
            itemIcon.DeactivateSound = null;
            itemIcon.DetailedCanActivate += CanActivate;
            itemIcon.OnKeyDown = false;
            itemIcon.SecondaryTechTypes.Add(UltimateWarpChip.thisTechType);
            itemIcon.activationType = ActivatedEquippableItem.ActivationType.OnceOff;
            itemIcon.AutoIconFade = false;
            Registries.RegisterHudItemIcon(itemIcon);

            chargingIcon = new ChargableEquippableItem("WarpChargeIcon", null, WarpChipItem.thisTechType);
            chargingIcon.ChargingReleasedSound = teleportSound;
            chargingIcon.ChargingStartSound = null;
            chargingIcon.SecondaryTechTypes.Add(UltimateWarpChip.thisTechType);
            chargingIcon.ShouldMakeIcon = false;
            chargingIcon.activateKey = QMod.config.ControlKey;
            chargingIcon.AutoIconFade = false;
            chargingIcon.IsIconActive += () => false;
            chargingIcon.ReleasedCharging += ReturnToBase;
            chargingIcon.MinChargeRequiredToTrigger = chargingIcon.MaxCharge;
            chargingIcon.AutoReleaseOnMaxCharge = true;

            chargingIcon.container = itemIcon.container;
            chargingIcon.itemIconObject = itemIcon.itemIconObject;
            chargingIcon.itemIcon = itemIcon.itemIcon;

            Registries.RegisterHudItemIcon(chargingIcon);


        }
        public void Update()
        {
            if (chargingIcon.charge >= 5f) chargingIcon.UpdateFill();
            else itemIcon.UpdateFill();
        }

        public void TryTeleport(List<TechType> techTypes)
        {
            if(justTeleportedToBase)
            {
                justTeleportedToBase = false;
                return;
            }
            if(player != null && !player.isPiloting && player.mode == Player.Mode.Normal)
            {
                Teleport(techTypes.Contains(UltimateWarpChip.thisTechType));
            }
        }

        public void Teleport(bool ultimateChipEquipped)
        {
            float maxDistance = player.IsInside() ? teleportDistanceInside : teleportDistanceOutside;

            float distance = maxDistance;

            if (Targeting.GetTarget(player.gameObject, maxDistance, out var _, out float wallDistance))
            {
                distance = wallDistance - teleportWallOffset;
            }

            Transform aimingTransform = player.camRoot.GetAimingTransform();
            player.SetPosition(player.transform.position + aimingTransform.forward * distance);

            CoroutineHost.StartCoroutine(TeleportFX());

            if (itemIcon != null)
            {
                if (!ultimateChipEquipped)
                    itemIcon.charge -= Mathf.Lerp(0f, itemIcon.MaxCharge, (100f / (maxDistance / distance)) / 100f);
                else
                    itemIcon.charge -= Mathf.Lerp(0f, itemIcon.MaxCharge, (100f / (maxDistance / distance)) / 100f) / 2f;
            }
        }

        public static IEnumerator TeleportFX(float delay = 0.25f)
        {
            TeleportScreenFXController fxController = MainCamera.camera.GetComponent<TeleportScreenFXController>();
            fxController.StartTeleport();
            yield return new WaitForSeconds(delay);
            fxController.StopTeleport();
        }
        public bool CanActivate(List<TechType> techTypes)
        {
            if (!techTypes.Contains(UltimateWarpChip.thisTechType))
                return itemIcon.charge == itemIcon.MaxCharge && player != null && !player.isPiloting && player.mode == Player.Mode.Normal;
            return itemIcon.charge >= itemIcon.MaxCharge / 2 && player != null && !player.isPiloting && player.mode == Player.Mode.Normal;
        }

        public bool IsChargeIconActive()
        {
            return chargingIcon.charge > 5f;
        }
        public void ReturnToBase()
        {
            if(player.currentSub && player.CheckSubValid(player.currentSub))
            {
                RespawnPoint respawn = player.currentSub.gameObject.GetComponentInChildren<RespawnPoint>();
                if (respawn)
                {
                    player.SetPosition(respawn.GetSpawnPosition());
                    justTeleportedToBase = true;
                    CoroutineHost.StartCoroutine(TeleportFX(1));
                    return;
                }
            }

            if (player.lastValidSub && player.CheckSubValid(player.lastValidSub))
            {
                RespawnPoint respawn = player.lastValidSub.gameObject.GetComponentInChildren<RespawnPoint>();
                if (respawn)
                {
                    player.SetPosition(respawn.GetSpawnPosition());
                    player.SetCurrentSub(player.lastValidSub);
                    justTeleportedToBase = true; 
                    CoroutineHost.StartCoroutine(TeleportFX(1));
                    return;
                }
            }
            ErrorMessage.AddMessage("Teleport failed, no safe location found");
        }
    }
}
