using CameraDroneUpgrades.API;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UWE;

namespace CameraDroneFlightUpgrade
{
    internal class FlightFunctionality
    {
        public CameraDroneUpgrade upgrade;

        public static float originalGrav = 9.81f;

        public bool hoverActive = false;
        private bool hoverSoundPlaying = false;
        public static float PowerDrain => QMod.config.powerDrain;

        private static readonly FMODAsset hoverLoop = GetFmodAsset("event:/sub/seamoth/seamoth_loop");
        private FMOD_CustomLoopingEmitter hoverEmitter;
        public static FMODAsset GetFmodAsset(string audioPath)
        {
            FMODAsset asset = ScriptableObject.CreateInstance<FMODAsset>();
            asset.path = audioPath;
            return asset;
        }
        private FMOD_CustomLoopingEmitter AddLoopingEmitter(FMODAsset asset)
        {
            var emitter = upgrade.camera.gameObject.AddComponent<FMOD_CustomLoopingEmitter>();
            emitter.SetAsset(asset);
            emitter.followParent = true;
            emitter.restartOnPlay = false;
            return emitter;
        }


        public void SetUp()
        {
            upgrade.equippedUpdate += EquippedUpdate;
            upgrade.activate += Activate;
            upgrade.deactivate += Deactivate;
            upgrade.key = QMod.config.flightKey;

            hoverEmitter = AddLoopingEmitter(hoverLoop);
            originalGrav = upgrade.camera.gameObject.GetComponent<WorldForces>().aboveWaterGravity;
        }
        public void EquippedUpdate()
        {
            if (hoverActive && upgrade.camera.gameObject.transform.position.y >= upgrade.camera.worldForces.waterDepth)
            {
                upgrade.camera.energyMixin.ModifyCharge(-PowerDrain * Time.deltaTime);
                StartHoverSound();
            }
            else
            {
                StopHoverSound();
            }
        }
        public void Activate()
        {
            hoverActive = true;
            upgrade.camera.gameObject.GetComponent<WorldForces>().aboveWaterGravity = 0;
        }
        public void Deactivate()
        {
            hoverActive = false;
            upgrade.camera.gameObject.GetComponent<WorldForces>().aboveWaterGravity = originalGrav;
        }
        public void StartHoverSound()
        {
            if(hoverEmitter && !hoverSoundPlaying)
                hoverEmitter.Play();
            hoverSoundPlaying = true;
        }

        public void StopHoverSound()
        {
            if (hoverEmitter && hoverSoundPlaying)
                hoverEmitter.Stop();
            hoverSoundPlaying = false;
        }
    }
}
