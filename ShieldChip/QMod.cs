using System.Reflection;
using HarmonyLib;
using QModManager.API.ModLoading;
using Logger = QModManager.Utility.Logger;
using UnityEngine;
using SMLHelper.V2.Json;
using SMLHelper.V2.Options.Attributes;
using SMLHelper.V2.Handlers;
using System.IO;

namespace ShieldSuit
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

            new ShieldSuitItem().Patch();

            Logger.Log(Logger.Level.Info, "Patched successfully!");
        }
    }
    [Menu("Shield Suit")]
    public class Config : ConfigFile
    {
        [Keybind("Shield Suit Key", Tooltip = "Press this key while you have a Shield Suit equipped to toggle it")]
        public KeyCode ShieldSuitKey = KeyCode.F;
    }
}
