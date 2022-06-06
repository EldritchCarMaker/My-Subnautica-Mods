using System.Reflection;
using HarmonyLib;
using QModManager.API.ModLoading;
using Logger = QModManager.Utility.Logger;
using UnityEngine;
using SMLHelper.V2.Json;
using SMLHelper.V2.Options.Attributes;
using SMLHelper.V2.Handlers;
using System.IO;
using System.Collections.Generic;
using SMLHelper.V2.Assets;
using CameraDroneSpeedUpgrade.Items;
using CameraDroneUpgrades.API;

namespace CameraDroneSpeedUpgrade
{
    [QModCore]
    public static class QMod
    {
        private static Assembly assembly = Assembly.GetExecutingAssembly();
        private static string modPath = Path.GetDirectoryName(assembly.Location);
        public static Config config { get; } = OptionsPanelHandler.Main.RegisterModOptions<Config>();

        [QModPatch]
        public static void Patch()
        {
            var CyclopsLockers = ($"Nagorogan_{assembly.GetName().Name}");
            Logger.Log(Logger.Level.Info, $"Patching {CyclopsLockers}");
            Harmony harmony = new Harmony(CyclopsLockers);
            harmony.PatchAll(assembly);

            var item = new MapRoomCameraSpeedUpgrade();
            item.Patch();

            var Speed = new SpeedFunctionality();
            Speed.upgrade = Registrations.RegisterDroneUpgrade("DroneSpeedUpgrade", item.TechType, Speed.SetUp);

            Logger.Log(Logger.Level.Info, "Patched successfully!");
        }
    }
    [Menu("Camera Drone Speed Upgrade")]
    public class Config : ConfigFile
    {
        [Keybind("Speed key", Tooltip = "keybind for using speed boost for drones")]
        public KeyCode speedKey = KeyCode.F;
    }
}
