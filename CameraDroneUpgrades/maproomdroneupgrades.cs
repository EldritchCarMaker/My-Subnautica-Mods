using CameraDroneUpgrades.API;
using SMLHelper.V2.Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Logger = QModManager.Utility.Logger;

namespace CameraDroneUpgrades
{
    internal class maproomdroneupgrades : MonoBehaviour
    {
        public MapRoomCamera camera;
        public MapRoomScreen screen;
        public MapRoomFunctionality functionality;
        public List<TechType> equippedUpgrades = new List<TechType>();
        public void Awake()
        {
            camera = GetComponent<MapRoomCamera>();
            screen = camera?.screen;
            functionality = screen?.mapRoomFunctionality;
            if(functionality == null)
            {
                Destroy(this);
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
            equippedUpgrades.Clear();
            foreach(CameraDroneUpgrade upgrade in Registrations.upgrades)
            {
                if(functionality.storageContainer.container.Contains(upgrade.techType))
                {
                    equippedUpgrades.Add(upgrade.techType);
                }
            }
        }
        public void Update()
        {
            foreach(CameraDroneUpgrade upgrade in Registrations.upgrades)
            {
                upgrade.update?.Invoke();
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
