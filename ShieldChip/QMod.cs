using System.Reflection;
using HarmonyLib;
using QModManager.API.ModLoading;
using Logger = QModManager.Utility.Logger;
using UnityEngine;
using SMLHelper.V2.Json;
using SMLHelper.V2.Options.Attributes;
using SMLHelper.V2.Handlers;
using System.IO;

namespace ShieldChip
{
    [QModCore]
    public static class QMod
    {
        internal static Config config { get; } = OptionsPanelHandler.Main.RegisterModOptions<Config>();
        [QModPatch]
        public static void Patch()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var CyclopsLockers = ($"Nagorogan_{assembly.GetName().Name}");
            Logger.Log(Logger.Level.Info, $"Patching {CyclopsLockers}");
            Harmony harmony = new Harmony(CyclopsLockers);
            harmony.PatchAll(assembly);

            new ShieldChipItem().Patch();

            Logger.Log(Logger.Level.Info, "Patched successfully!");
        }
    }
    [Menu("ShieldChip")]
    public class Config : ConfigFile
    {
        [Keybind("Shield Chip Key", Tooltip = "Press this key while you have a Shield Chip equipped to toggle it")]
        public KeyCode ShieldChipKey = KeyCode.F;
    }
}
