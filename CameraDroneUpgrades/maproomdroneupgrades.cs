using CameraDroneUpgrades.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using CameraDroneUpgrades.DroneScanning;
using UWE;

namespace CameraDroneUpgrades
{
    public class maproomdroneupgrades : MonoBehaviour
    {
        public MapRoomCamera camera { get; internal set; }
        public MapRoomScreen screen { get; internal set; }
        public MapRoomFunctionality functionality { get; internal set; }

        public static maproomdroneupgrades currentManager { get; internal set; }

        public readonly List<TechType> equippedUpgrades = new List<TechType>();//don't touch

        public void Awake()
        {
            if(currentManager != null) Destroy(currentManager);

            currentManager = this;

            ScanFunctionality.scanEmitter= ScanFunctionality.AddLoopingEmitter(ScanFunctionality.scanLoop, gameObject);

            camera = GetComponent<MapRoomCamera>();
            screen = camera?.screen;
            functionality = screen?.mapRoomFunctionality;
            if(functionality == null)
            {
                Destroy(this);
                return;
            }
            CountUpgrades(null);
            functionality.storageContainer.container.onAddItem += CountUpgrades;
            functionality.storageContainer.container.onRemoveItem += CountUpgrades;

            foreach(CameraDroneUpgrade upgrade in Registrations.upgrades)
            {
                upgrade.camera = camera;
                upgrade.setUp?.Invoke();
            }
        }
        public void CountUpgrades(InventoryItem _)
        {
            if (functionality == null) return;

            equippedUpgrades.Clear();
            foreach(CameraDroneUpgrade upgrade in Registrations.upgrades)
            {
                if(functionality.storageContainer.container.Contains(upgrade.techType))
                {
                    equippedUpgrades.Add(upgrade.techType);
                }
            }
        }
        public void OnExitCamera()
        {
            foreach (CameraDroneUpgrade upgrade in Registrations.upgrades)
            {
                if (upgrade.active)
                {
                    upgrade.deactivate?.Invoke();
                    upgrade.active = false;
                }
                upgrade.update?.Invoke();
                upgrade.unEquip?.Invoke();
            }
            uGUI_ScannerIcon.main.icon.backgroundColorNormal = ScanFunctionality.vanillaColor;
        }
        public void Update()
        {
            if(camera.IsControlled())
            {
                var obj = ScanFunctionality.UpdateScanIcon(gameObject);
                if (obj && Input.GetKey(QMod.Config.scanKey) && ScanFunctionality.Scan(gameObject, obj))
                {
                    camera.energyMixin.ConsumeEnergy(0.5f * Time.deltaTime);
                }
            }

            ScanFunctionality.UpdateSounds();

            foreach (CameraDroneUpgrade upgrade in Registrations.upgrades)
            {
                upgrade.update?.Invoke();

                if(!camera.IsControlled())
                {
                    if(upgrade.active)
                    {
                        upgrade.deactivate?.Invoke();
                        upgrade.active = false;
                    }
                    continue;
                }

                if(equippedUpgrades.Contains(upgrade.techType))
                {
                    upgrade.equippedUpdate?.Invoke();
                    if(upgrade.activationType == CameraDroneUpgrade.ActivationType.OnceOff)
                    {
                        if (Input.GetKeyDown(upgrade.key))
                        {
                            upgrade.active = !upgrade.active;
                            if(upgrade.active)
                                upgrade.activate?.Invoke();
                            else
                                upgrade.deactivate?.Invoke();
                        }
                        continue;
                    }
                    if (upgrade.activationType == CameraDroneUpgrade.ActivationType.Held && Input.GetKey(upgrade.key))
                    {
                        upgrade.activate?.Invoke();
                        upgrade.active = true;
                    }
                    else if(upgrade.active)
                    {
                        upgrade.deactivate?.Invoke();
                        upgrade.active = false;
                    }
                }
            }
        }
    }
}
