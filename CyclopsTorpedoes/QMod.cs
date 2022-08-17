using System.Reflection;
using HarmonyLib;
using QModManager.API.ModLoading;
using Logger = QModManager.Utility.Logger;

using SMLHelper.V2.Json;
using SMLHelper.V2.Options.Attributes;
using SMLHelper.V2.Handlers;
using UnityEngine;
using System.Collections.Generic;
using MoreCyclopsUpgrades.API;

namespace CyclopsTorpedoes
{
    [QModCore]
    public static class QMod
    {
        internal static Config config { get; } = OptionsPanelHandler.Main.RegisterModOptions<Config>();
        [QModPatch]
        public static void Patch()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var stingers = ($"Nagorogan_{assembly.GetName().Name}");
            Logger.Log(Logger.Level.Info, $"Patching {stingers}");
            Harmony harmony = new Harmony(stingers);
            harmony.PatchAll(assembly);

            var module = new TorpedoModule();
            module.Patch();

            MCUServices.Register.CyclopsUpgradeHandler((SubRoot cyclops) => 
            { 
                return new MoreCyclopsUpgrades.API.Upgrades.UpgradeHandler(module.TechType, cyclops);
            });

            Logger.Log(Logger.Level.Info, "Patched successfully!");
        }
    }
    [Menu("Cyclops Torpedoes")]
    public class Config : ConfigFile
    {
        [Keybind("Torpedo Key", Tooltip = "Press this key while you have the Burst Fins equipped to activate them")]
        public KeyCode torpedoKey = KeyCode.F;
        public TechType priorityTorpedoType = TechType.GasTorpedo;
        public Dictionary<string, int> torpedoTypePriority = new Dictionary<string, int>();
    }
}