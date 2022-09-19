using System.Reflection;
using HarmonyLib;
using QModManager.API.ModLoading;
using Logger = QModManager.Utility.Logger;
using UnityEngine;
using SMLHelper.V2.Json;
using SMLHelper.V2.Options.Attributes;
using SMLHelper.V2.Handlers;
using System.IO;

namespace TimeControlSuit
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

            new TimeSuitItem().Patch();

            Logger.Log(Logger.Level.Info, "Patched successfully!");
        }
    }
    [Menu("Time Control Suit")]
    public class Config : ConfigFile
    {
        [Keybind("Shield Suit Key", Tooltip = "Press this key while you have a Time Suit equipped to activate it")]
        public KeyCode TimeSuitKey = KeyCode.F;
    }
}
