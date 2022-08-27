﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;

namespace ECMModLogger
{
    [HarmonyPatch(typeof(MainMenuRightSide))]
    internal class InGameMenuPatches
    {
        public static Dictionary<string, string> modListIDToFriendly = new Dictionary<string, string>
        {
            { "AdaptiveTeleportingCosts", "Adaptive Teleporting Costs" },
            { "BetterCyclopsLockers", "Better Cyclops Lockers"},
            { "BurstFins", "Burst Fins"},
            {"CameraDroneDefenseUpgrade", "Camera Drone Defense Upgrade"},
            {"CameraDroneFlightUpgrade", "Camera Drone Flight Upgrade"},
            { "CameraDroneRepairUpgrade", "Camera Drone Repair Upgrade"},
            { "CameraDroneShieldUpgrade", "Camera Drone Shield Upgrade"},
            { "CameraDroneSpeedUpgrade", "Camera Drone Speed Upgrade"},
            { "CameraDroneStasisUpgrade", "Camera Drone Stasis Upgrade"},
            { "CameraDroneStealthUpgrade", "Camera Drone Stealth Upgrade"},
            { "CameraDroneUpgrades", "Camera Drone Upgrades"},
            { "CyclopsTorpedoes", "Cyclops Torpedoes"},
            { "CyclopsVehicleUpgradeConsole", "Cyclops Vehicle Upgrade Console"},
            { "CyclopsWindows", "Cyclops Windows"},
            { "DroopingStingersNerf", "Drooping Stingers Nerf"},
            { "EquippableItemIcons", "Equippable Item Icons"},
            { "EquivalentExchange", "Equivalent Exchange"},
            { "InvincibleDockedVehicles", "Invincible Docked Vehicles"},
            { "PickupableVehicles", "Pickupable Vehicles"},
            { "PlayerToolChanges_SN", "Player Tool Changes SN"},
            { "RechargerChips", "Recharger Chips"},
            { "RemoteControlVehicles", "Remote Control Vehicles"},
            { "SeedsFromHarvesting", "Seeds From Harvesting"},
            { "ShieldSuit", "Shield Suit"},
            { "SonarChip", "Sonar Chip"},
            { "SoundCommands", "Sound Commands"},
            { "SpyWatch", "Spy Watch"},
            { "WarpChip", "Warp Chip"},
            { "CameraDroneSonarMod", "Camera Drone Sonar Mod" },
            { "cyclopsVehiclebayHUDIcon", "cyclops Vehicle bay HUD Icon" },
            { "CyclopsHR", "Cyclops Hull Reinforcement" },
            { "moreEngineEfficiencyModules", "More Engine Efficiency Modules" },

        };
        public static Dictionary<string, string> specialModsList = new Dictionary<string, string>
        {
            { "CyclopsCameraDroneMod", "Seriously, install Cyclops Camera Drones. They got lasers and tractor beams and shit." },
        };
        [HarmonyPatch(nameof(MainMenuRightSide.Start))]
        public static void Postfix()
        {
            foreach(KeyValuePair<string, string> pair in modListIDToFriendly)
            {
                if(!QModManager.API.QModServices.Main.ModPresent(pair.Key))
                    ErrorMessage.AddWarning($"{pair.Value} not found! Please Install it to further enhance your Subnautica experience!");
            }
            foreach(KeyValuePair<string, string> pair in specialModsList)
            {
                if (!QModManager.API.QModServices.Main.ModPresent(pair.Key))
                    ErrorMessage.AddWarning($"{pair.Key} not found! Please Install it to further enhance your Subnautica experience! {pair.Value}");
            }
        }
    }
}
