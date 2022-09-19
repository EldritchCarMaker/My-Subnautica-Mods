using System.Reflection;
using HarmonyLib;
using QModManager.API.ModLoading;
using Logger = QModManager.Utility.Logger;
using SMLHelper.V2.Json;
using SMLHelper.V2.Options.Attributes;
using SMLHelper.V2.Handlers;
using UnityEngine;
using WarpVehicleModule.Items;
using UWE;
using System.Collections;

namespace WarpVehicleModule
{
    [QModCore]
    public static class QMod
    {
        internal static Config config { get; } = OptionsPanelHandler.Main.RegisterModOptions<Config>();
        [QModPatch]
        public static void Patch()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var name = ($"Nagorogan_{assembly.GetName().Name}");
            Logger.Log(Logger.Level.Info, $"Patching {name}");
            Harmony harmony = new Harmony(name);
            harmony.PatchAll(assembly);

            WarpVehicleItem warpChipItem = new WarpVehicleItem();
            warpChipItem.Patch();

            Logger.Log(Logger.Level.Info, "Patched successfully!");
        }
    }
    [Menu("WarpVehicleModule")]
    public class Config : ConfigFile
    {
        public int DefaultWarpDistanceOutside = 15;
        public int DefaultWarpDistanceInside = 10;
        public float DefaultWarpCooldown = 5;
    }
}
