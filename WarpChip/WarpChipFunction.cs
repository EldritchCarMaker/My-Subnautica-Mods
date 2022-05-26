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
        //public bool UpgradedItemEquipped = false;
        public int FramesSinceCheck = 0;

        public void Awake()
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
            itemIcon.SecondaryTechTypes.Add(UltimateWarpChip.thisTechType);
            itemIcon.activationType = ActivatedEquippableItem.ActivationType.OnceOff;
            Registries.RegisterHudItemIcon(itemIcon);

            player = GetComponent<Player>();
        }
        public void UpdateEquipped(string slot, InventoryItem item)
        {
            //UpgradedItemEquipped = Utility.EquipmentHasItem(UltimateWarpChip.thisTechType);
        }
        public void Start()
        {
            Inventory.main.equipment.onEquip += UpdateEquipped;
            Inventory.main.equipment.onUnequip += UpdateEquipped;
            UpdateEquipped(null, null);
        }

        public void TryTeleport(List<TechType> techTypes)
        {
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

            TeleportScreenFXController fxController = MainCamera.camera.GetComponent<TeleportScreenFXController>();
            fxController.StartTeleport();
            CoroutineHost.StartCoroutine(TeleportFX());

            if (itemIcon != null)
            {
                if (!ultimateChipEquipped)
                    itemIcon.charge -= Mathf.Lerp(0f, itemIcon.MaxCharge, (100f / (maxDistance / distance)) / 100f);//fix
                else//don't work right when not going full distance. Even when going half, just uses full charge
                    itemIcon.charge -= Mathf.Lerp(0f, itemIcon.MaxCharge, (100f / (maxDistance / distance)) / 100f) / 2f;//fix
            }
        }

        public static IEnumerator TeleportFX()
        {
            TeleportScreenFXController fxController = MainCamera.camera.GetComponent<TeleportScreenFXController>();
            yield return new WaitForSeconds(0.25f);
            fxController.StopTeleport();
        }
        public bool CanActivate(List<TechType> techTypes)
        {
            if (!techTypes.Contains(UltimateWarpChip.thisTechType))
                return itemIcon.charge == itemIcon.MaxCharge && player != null && !player.isPiloting && player.mode == Player.Mode.Normal;
            return itemIcon.charge >= itemIcon.MaxCharge / 2 && player != null && !player.isPiloting && player.mode == Player.Mode.Normal;
        }
    }
}
