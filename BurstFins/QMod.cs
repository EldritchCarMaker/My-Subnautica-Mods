using System.Reflection;
using HarmonyLib;
using QModManager.API.ModLoading;
using Logger = QModManager.Utility.Logger;
using UnityEngine;
using SMLHelper.V2.Json;
using SMLHelper.V2.Options.Attributes;
using SMLHelper.V2.Handlers;
using System.IO;

namespace BurstFins
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

            new BurstFinsItem().Patch();

            Logger.Log(Logger.Level.Info, "Patched successfully!");
        }
    }
    [Menu("BurstFins")]
    public class Config : ConfigFile
    {
        [Keybind("Burst Fins Key", Tooltip = "Press this key while you have the Burst Fins equipped to activate them")]
        public KeyCode BurstFinsKey = KeyCode.F;
        [Toggle("Held", Tooltip = "Toggles whether the activation key should be pressed or held to use the fins")]
        public bool HeldKey = true;
    }
}
