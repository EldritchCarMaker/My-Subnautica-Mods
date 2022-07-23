using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CameraDroneUpgrades.DroneScanning
{
    internal static class ScanFunctionality
    {
        public static float timeLastScan;
        public static Color vanillaColor;

        private static readonly FMODAsset scanEndSound = GetFmodAsset("event:/tools/scanner/scan_complete");
        internal static readonly FMODAsset scanLoop = GetFmodAsset("event:/tools/scanner/scan_loop");
        private static bool soundPlaying = false;
        internal static FMOD_CustomLoopingEmitter scanEmitter;

        public static FMODAsset GetFmodAsset(string audioPath)
        {
            FMODAsset asset = ScriptableObject.CreateInstance<FMODAsset>();
            asset.path = audioPath;
            return asset;
        }
        public static FMOD_CustomLoopingEmitter AddLoopingEmitter(FMODAsset asset, GameObject gameObject)
        {
            var emitter = gameObject.AddComponent<FMOD_CustomLoopingEmitter>();
            emitter.SetAsset(asset);
            emitter.followParent = true;
            emitter.restartOnPlay = false;
            return emitter;
        }


        public static GameObject UpdateScanIcon(GameObject startObj)
        {
            if (!Targeting.GetTarget(startObj, 5, out var go, out float distance)) return null;
            UpdateScanTarget(go); 

            if (PDAScanner.scanTarget.isValid && PDAScanner.CanScan() == PDAScanner.Result.Scan && !PDAScanner.scanTarget.isPlayer)
            {
                float progress = PDAScanner.scanTarget.progress;
                Color endColor = new Color(1, 0, 0);

                Color color = Color.Lerp(vanillaColor, endColor, progress);
                ScannerIconFunction(1 - progress, color);
            }

            return go;
        }
        public static bool Scan(GameObject startObj, GameObject targetObj)
        {
            //bool is for whether energy should be drained or not. In other words, if the scan actually did anything

            PDAScanner.ScanTarget scanTarget = PDAScanner.scanTarget;
            if (scanTarget.isValid)
            {
                PDAScanner.Result result = PDAScanner.Scan();
                if (result == PDAScanner.Result.Scan)
                {
                    timeLastScan = Time.time;
                    return true;
                }
                else if (result == PDAScanner.Result.Done || result == PDAScanner.Result.Researched)
                {
                    PlayScanEndSound(startObj.transform);
                }
            }
            /*doesn't work well, ignore this sadly
            var terminal = gameObject1.GetComponentInParent<StoryHandTarget>();
            if (terminal != null)
            {
                HandleEnergyDrain(mapRoomCamera, 0.5f * Time.deltaTime);
                terminal.OnHandClick(Player.main.armsController.guiHand);
            }
            */
            var blueprint = targetObj.GetComponentInParent<BlueprintHandTarget>();
            if (blueprint != null)
            {
                blueprint.UnlockBlueprint();
                return true;
            }
            return false;
        }
        public static void UpdateScanTarget(GameObject objTarget)
        {
            PDAScanner.ScanTarget newTarget = default;
            newTarget.Invalidate();

            // Finds the correct object and sets the gameObject, uid and the techType fields of ScanTarget to the appropriate values.
            newTarget.Initialize(objTarget);

            // If the previous scan target is the same as the new one, exit early.
            if (PDAScanner.scanTarget.techType == newTarget.techType &&
                      PDAScanner.scanTarget.gameObject == newTarget.gameObject &&
                      PDAScanner.scanTarget.uid == newTarget.uid) return;

            // Finds the cached progress of this item and if it exists, we set it as our new scan target's progress
            if (newTarget.hasUID && PDAScanner.cachedProgress.TryGetValue(newTarget.uid, out var progress))
            {
                newTarget.progress = progress;
            }

            // Set the scanner's target to the new target
            PDAScanner.scanTarget = newTarget;
        }
        public static void ScannerIconFunction(float alpha, Color color)
        {
            uGUI_ScannerIcon icon = uGUI_ScannerIcon.main;
            if (icon == null) return;
            icon.Show();
            icon.icon.SetBackgroundColors(color, color, color);
        }

        public static void PlayScanEndSound(Transform transform)
        {
            Utils.PlayFMODAsset(scanEndSound, transform.position);
        }

        public static void UpdateSounds()
        {
            if (Time.time <= timeLastScan + 0.5f)
                StartScanLoopSound();
            else 
                StopScanLoopSound();
        }
        public static void StartScanLoopSound()
        {
            if (!soundPlaying && scanEmitter)
            {
                scanEmitter.Play();
                soundPlaying = true;
            }
        }
        public static void StopScanLoopSound()
        {
            if (soundPlaying && scanEmitter)
            {
                scanEmitter.Stop();
                soundPlaying = false;
            }
        }
    }
}
