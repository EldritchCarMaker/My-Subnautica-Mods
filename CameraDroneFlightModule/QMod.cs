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
using CameraDroneFlightUpgrade.Items;
using CameraDroneUpgrades.API;

namespace CameraDroneFlightUpgrade
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
            var CyclopsLockers = ($"EldritchCarMaker_{assembly.GetName().Name}");
            Logger.Log(Logger.Level.Info, $"Patching {CyclopsLockers}");
            Harmony harmony = new Harmony(CyclopsLockers);
            harmony.PatchAll(assembly);

            var item = new MapRoomCameraFlightUpgrade();
            item.Patch();

            var shield = new FlightFunctionality();
            shield.upgrade = Registrations.RegisterDroneUpgrade("DroneFlightUpgrade", item.TechType, shield.SetUp);

            Logger.Log(Logger.Level.Info, "Patched successfully!");
        }
    }
    [Menu("Camera Drone Flight Upgrade")]
    public class Config : ConfigFile
    {
        [Keybind("Flight", Tooltip = "keybind for toggling hover mode while using camera drones")]
        public KeyCode flightKey = KeyCode.H;
    }
}
