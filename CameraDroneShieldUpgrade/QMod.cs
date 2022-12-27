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
using CameraDroneShieldUpgrade.Items;
using CameraDroneUpgrades.API;

namespace CameraDroneShieldUpgrade
{
    [QModCore]
    public static class QMod
    {
        private static Assembly assembly = Assembly.GetExecutingAssembly();
        private static string modPath = Path.GetDirectoryName(assembly.Location);
        public static Config config { get; } = OptionsPanelHandler.Main.RegisterModOptions<Config>();

        public static ShieldFunctionality shield;

        [QModPatch]
        public static void Patch()
        {
            var CyclopsLockers = ($"EldritchCarMaker_{assembly.GetName().Name}");
            Logger.Log(Logger.Level.Info, $"Patching {CyclopsLockers}");
            Harmony harmony = new Harmony(CyclopsLockers);
            harmony.PatchAll(assembly);

            var item = new MapRoomCameraShieldUpgrade();
            item.Patch();

            shield = new ShieldFunctionality();
            shield.upgrade = Registrations.RegisterDroneUpgrade("DroneShieldUpgrade", item.TechType, shield.SetUp);

            Logger.Log(Logger.Level.Info, "Patched successfully!");
        }
    }
    [Menu("Camera Drone Shield Upgrade")]
    public class Config : ConfigFile
    {
        [Keybind("Shield", Tooltip = "keybind for toggling a shield for camera drones")]
        public KeyCode shieldKey = KeyCode.X;
    }
}
