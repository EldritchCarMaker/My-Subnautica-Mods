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
using CameraDroneStealthUpgrade.Items;
using CameraDroneUpgrades.API;

namespace CameraDroneStealthUpgrade
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

            var item = new MapRoomCameraStealthUpgrade();
            item.Patch();

            var Stealth = new StealthFunctionality();
            Stealth.upgrade = Registrations.RegisterDroneUpgrade("DroneStealthUpgrade", item.TechType, Stealth.SetUp);

            Logger.Log(Logger.Level.Info, "Patched successfully!");
        }
    }
    [Menu("Camera Drone Stealth Upgrade")]
    public class Config : ConfigFile
    {
        [Keybind("Stealth mode", Tooltip = "keybind for toggling stealth for camera drones")]
        public KeyCode stealthKey = KeyCode.T;
    }
}
