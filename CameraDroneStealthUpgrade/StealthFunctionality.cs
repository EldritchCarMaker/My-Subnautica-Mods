using CameraDroneUpgrades.API;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UWE;

namespace CameraDroneStealthUpgrade
{
    internal class StealthFunctionality
    {
        public CameraDroneUpgrade upgrade;

        private FMODAsset startStealth = GetFmodAsset("event:/sub/cyclops/install_mod");
        private FMODAsset stopStealth = GetFmodAsset("event:/tools/builder/remove");
        public static FMODAsset GetFmodAsset(string audioPath)
        {
            FMODAsset asset = ScriptableObject.CreateInstance<FMODAsset>();
            asset.path = audioPath;
            return asset;
        }
        public void SetUp()
        {
            upgrade.activate += Activate;
            upgrade.deactivate += Deactivate;
            upgrade.unEquip += UnEquip;
            upgrade.key = QMod.config.stealthKey;
        }
        public void UnEquip()
        {
            upgrade.camera.GetComponent<EcoTarget>()?.SetTargetType(EcoTargetType.Shiny);
        }
        public void Deactivate()
        {
            upgrade.camera.GetComponent<EcoTarget>()?.SetTargetType(EcoTargetType.Shiny);
            Utils.PlayFMODAsset(stopStealth);
        }
        public void Activate()
        {
            upgrade.camera.GetComponent<EcoTarget>()?.SetTargetType(EcoTargetType.Leviathan);
            Utils.PlayFMODAsset(startStealth);
        }
    }
}
