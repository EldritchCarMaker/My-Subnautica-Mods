using System.Reflection;
using HarmonyLib;
using QModManager.API.ModLoading;
using Logger = QModManager.Utility.Logger;
using UnityEngine;
using SMLHelper.V2.Json;
using SMLHelper.V2.Options.Attributes;
using SMLHelper.V2.Handlers;
using System.IO;

namespace ArmorSuit
{
    [QModCore]
    public static class QMod
    {
        internal static Config config { get; } = OptionsPanelHandler.Main.RegisterModOptions<Config>();
        [QModPatch]
        public static void Patch()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var CyclopsLockers = ($"EldritchCarMaker_{assembly.GetName().Name}");
            Logger.Log(Logger.Level.Info, $"Patching {CyclopsLockers}");
            Harmony harmony = new Harmony(CyclopsLockers);
            harmony.PatchAll(assembly);

            new ArmorSuitItem().Patch();
            new ArmorGlovesItem().Patch();

            Logger.Log(Logger.Level.Info, "Patched successfully!");
        }
    }
    [Menu("ArmorSuit")]
    public class Config : ConfigFile
    {
        [Keybind("Armor Suit Key", Tooltip = "Press this key while you have the the armor suit equipped to switch the current damage type")]
        public KeyCode ArmorSuitKey = KeyCode.X;
        [Toggle("Automatic", Tooltip = "Toggles whether the armor suit will automatically adapt to the last damage type taken, or if it will simply stay as the type specified")]
        public bool Automatic = true;
    }
}
