using CameraDroneUpgrades.API;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UWE;

namespace CameraDroneSpeedUpgrade
{
    internal class SpeedFunctionality
    {
        public CameraDroneUpgrade upgrade;

        public bool speedActive = false;
        private static readonly FMODAsset speedStart = GetFmodAsset("event:/sub/cyclops/install_mod");
        private static readonly FMODAsset speedEnd = GetFmodAsset("event:/tools/battery_die");
        private static readonly FMODAsset loopSound = GetFmodAsset("event:/sub/cyclops/cyclops_loop_fast");
        private static FMOD_CustomEmitter loop;

        public const float energyDrain = 0.5f;
        public const float speed = 10f;
        public static FMODAsset GetFmodAsset(string audioPath)
        {
            FMODAsset asset = ScriptableObject.CreateInstance<FMODAsset>();
            asset.path = audioPath;
            return asset;
        }
        private FMOD_CustomLoopingEmitter AddLoopingEmitter(FMODAsset asset)
        {
            var emitter = upgrade.camera?.gameObject.AddComponent<FMOD_CustomLoopingEmitter>();
            emitter.SetAsset(asset);
            emitter.followParent = true;
            emitter.restartOnPlay = false;
            return emitter;
        }

        public void SetUp()
        {
            upgrade.equippedUpdate += EquippedUpdate;
            upgrade.activate += Activate;
            upgrade.deactivate += Activate;
            upgrade.key = QMod.config.speedKey;
        }
        public void EquippedUpdate()
        {
            if(speedActive && upgrade.camera.transform.position.y < upgrade.camera.worldForces.waterDepth)
            {
                upgrade.camera.energyMixin.ConsumeEnergy(energyDrain * Time.deltaTime);
                upgrade.camera?.rigidBody.AddForce(upgrade.camera.transform.rotation * (speed * upgrade.camera.wishDir), ForceMode.Acceleration);
            }
        }
        public void Activate()
        {
            speedActive = !speedActive;
            if (speedActive)
                Utils.PlayFMODAsset(speedStart, upgrade.camera.transform);
            else 
                Utils.PlayFMODAsset(speedEnd, upgrade.camera.transform);

            if (loop == null) loop = AddLoopingEmitter(loopSound);

            if (speedActive)
                loop.Play();
            else
                loop.Stop();
        }
    }
}
