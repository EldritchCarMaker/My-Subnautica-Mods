using CameraDroneUpgrades.API;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UWE;

namespace CameraDroneStasisUpgrade
{
    internal class StasisFunctionality
    {
        public CameraDroneUpgrade upgrade;

        public const float energyDrain = 3; 
        
        StasisSphere sphere { get => StasisRifle.sphere; set => StasisRifle.sphere = value; }

        public void SetUp()
        {
            upgrade.activate += Activate;
            upgrade.deactivate += Activate;
            upgrade.unEquip += UnEquip;
            upgrade.key = QMod.config.stasisKey;

            CoroutineHost.StartCoroutine(SetSphere());
        }
        public IEnumerator SetSphere()
        {
            if (!sphere)
            {
                var task = CraftData.GetPrefabForTechTypeAsync(TechType.StasisRifle);
                yield return task;
                task.GetResult().GetComponent<StasisRifle>()?.Awake();
                sphere = StasisRifle.sphere;
            }
        }
        public void Activate()
        {
            if (!sphere)
            {
                QMod.Logger.LogWarning("Stasis sphere is still null, can't use stasis upgrade");
                ErrorMessage.AddMessage("Could not get stasis sphere, can't use upgrade");
                return;
            }

            sphere.Shoot(upgrade.camera.transform.position, upgrade.camera.transform.rotation, 0.5f, 0.5f, 1);
            sphere.EnableField();
            upgrade.camera.energyMixin.ConsumeEnergy(energyDrain);
        }
        public void UnEquip()
        {

        }
    }
}
