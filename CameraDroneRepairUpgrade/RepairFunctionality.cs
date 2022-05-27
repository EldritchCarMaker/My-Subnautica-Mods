using CameraDroneUpgrades.API;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UWE;
using Logger = QModManager.Utility.Logger;

namespace CameraDroneRepairUpgrade
{
    internal class RepairFunctionality
    {
        public CameraDroneUpgrade upgrade;



        private bool repairSoundPlaying = false;
        private float timeLastRepair = 0;
        public Color vanillaColor;
        private static readonly FMODAsset repairLoop = GetFmodAsset("event:/tools/welder/weld_loop");
        private FMOD_CustomLoopingEmitter repairEmitter;
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
            upgrade.deactivate += StopRepairSound;
            upgrade.key = QMod.config.repairKey;
            upgrade.activationType = CameraDroneUpgrade.ActivationType.Held;

            repairEmitter = AddLoopingEmitter(repairLoop);
            if(uGUI_ScannerIcon.main == null)
            {
                CoroutineHost.StartCoroutine(waitForIcon());
                return;
            }
            vanillaColor = uGUI_ScannerIcon.main.icon.backgroundColorNormal;
            uGUI_ScannerIcon.main.gameObject.AddComponent<uGUI_RepairUpgradeIcon>();
        }
        public IEnumerator waitForIcon()
        {
            yield return new WaitUntil(() => uGUI_ScannerIcon.main != null);
            vanillaColor = uGUI_ScannerIcon.main.icon.backgroundColorNormal;
            uGUI_ScannerIcon.main.gameObject.AddComponent<uGUI_RepairUpgradeIcon>();
        }
        public void EquippedUpdate()
        {
            if (!Targeting.GetTarget(upgrade.camera.gameObject, 5, out var gameObject, out float distance)) return;

            LiveMixin liveMixin = gameObject.GetComponentInParent<LiveMixin>();
            if (liveMixin != null && liveMixin.IsWeldable())
            {
                float healthPercentage = liveMixin.GetHealthFraction();

                Color startColor = new Color(1, 0, 0);

                Color color = Color.Lerp(startColor, vanillaColor, healthPercentage);
                RepairIconFunction(healthPercentage, color);
            }
        }
        public void Activate()
        {
            StartRepairSound();

            if(!Targeting.GetTarget(upgrade.camera.gameObject, 5, out var gameObject, out float distance)) return;

            LiveMixin liveMixin = gameObject.FindAncestor<LiveMixin>();
            if (liveMixin && Time.time >= timeLastRepair + 0.5f)
            {
                if (liveMixin.IsWeldable())
                {
                    liveMixin.AddHealth(10);
                    upgrade.camera.energyMixin.ModifyCharge(-0.5f * Time.deltaTime);
                    if (!liveMixin.IsFullHealth())
                    {
                        timeLastRepair = Time.time;
                    }
                }
                else
                {
                    WeldablePoint weldablePoint = gameObject.GetComponentInChildren<WeldablePoint>();
                    if (weldablePoint != null)
                    {
                        liveMixin.AddHealth(10);
                        upgrade.camera.energyMixin.ModifyCharge(-0.5f * Time.deltaTime);
                        if (!liveMixin.IsFullHealth())
                        {
                            timeLastRepair = Time.time;
                        }
                    }
                }
            }
        }
        public void StartRepairSound()
        {
            if(repairEmitter && !repairSoundPlaying)
                repairEmitter.Play();
            repairSoundPlaying = true;
        }

        public void StopRepairSound()
        {
            if (repairEmitter && repairSoundPlaying)
                repairEmitter.Stop();
            repairSoundPlaying = false;
        }

        /*public void PlayRepairEnd()
        {
            Utils.PlayFMODAsset(repairEndSound, transform.position);
        }*/
        public void RepairIconFunction(float alpha, Color color)
        {
            uGUI_RepairUpgradeIcon icon = uGUI_RepairUpgradeIcon.main;
            icon.Show();
            icon.SetAlpha(alpha);
            icon.icon.SetBackgroundColors(color, color, color);
        }
    }
}
