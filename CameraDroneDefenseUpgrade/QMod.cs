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
using CameraDroneDefenseUpgrade.Items;
using CameraDroneUpgrades.API;

namespace CameraDroneDefenseUpgrade
{
    [QModCore]
    public static class QMod
    {
        private static Assembly assembly = Assembly.GetExecutingAssembly();
        private static string modPath = Path.GetDirectoryName(assembly.Location);

        [QModPatch]
        public static void Patch()
        {
            var CyclopsLockers = ($"Nagorogan_{assembly.GetName().Name}");
            Logger.Log(Logger.Level.Info, $"Patching {CyclopsLockers}");
            Harmony harmony = new Harmony(CyclopsLockers);
            harmony.PatchAll(assembly);

            var item = new MapRoomCameraDefenseUpgrade();
            item.Patch();

            Registrations.RegisterDroneUpgrade("DroneDefenseUpgrade", item.TechType, null).key = KeyCode.None;

            Logger.Log(Logger.Level.Info, "Patched successfully!");
        }
    }
}
