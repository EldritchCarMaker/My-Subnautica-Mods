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
using CameraDroneRepairUpgrade.Items;
using CameraDroneUpgrades.API;

namespace CameraDroneRepairUpgrade
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

            var item = new MapRoomCameraRepairUpgrade();
            item.Patch();

            var shield = new RepairFunctionality();
            shield.upgrade = Registrations.RegisterDroneUpgrade("DroneRepairUpgrade", item.TechType, shield.SetUp);

            Logger.Log(Logger.Level.Info, "Patched successfully!");
        }
    }
    [Menu("Camera Drone Repair Upgrade")]
    public class Config : ConfigFile
    {
        [Keybind("Repair", Tooltip = "keybind for repairing objects while using camera drones")]
        public KeyCode repairKey = KeyCode.R;
    }
}
