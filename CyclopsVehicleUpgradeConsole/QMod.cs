using System.Reflection;
using HarmonyLib;
using QModManager.API.ModLoading;
using Logger = QModManager.Utility.Logger;

using SMLHelper.V2.Json;
using SMLHelper.V2.Options.Attributes;
using SMLHelper.V2.Handlers;
using SMLHelper.V2.Json.Attributes;

namespace CyclopsVehicleUpgradeConsole
{
    [QModCore]
    public static class QMod
    {
        public static Config config { get; set; } = OptionsPanelHandler.Main.RegisterModOptions<Config>();
        [QModPatch]
        public static void Patch()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var stingers = ($"EldritchCarMaker_{assembly.GetName().Name}");
            Logger.Log(Logger.Level.Info, $"Patching {stingers}");
            Harmony harmony = new Harmony(stingers);
            harmony.PatchAll(assembly);
            Logger.Log(Logger.Level.Info, "Patched successfully!");
        }
    }
    [Menu("Cyclops Vehicle Upgrade Console")]
    public class Config : ConfigFile
    {
        [Toggle("Hide Unknown Vehicles", Tooltip = "Hides the vehicle creation button if you haven't unlocked the vehicle yet")]
        public bool hideUnknown = false;
    }
}