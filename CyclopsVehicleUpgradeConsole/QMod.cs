using System.Reflection;
using HarmonyLib;
#if !SN2
using QModManager.API.ModLoading;
using Logger = QModManager.Utility.Logger;
using SMLHelper.V2.Json;
using SMLHelper.V2.Options.Attributes;
using SMLHelper.V2.Handlers;
using SMLHelper.V2.Json.Attributes;
#else
using BepInEx;
using Nautilus.Json;
using Nautilus.Handlers;
using Nautilus.Options.Attributes;
#endif

namespace CyclopsVehicleUpgradeConsole
{
#if !SN2
    [QModCore]
    public static class QMod
    {
        internal static Config config { get; } = OptionsPanelHandler.Main.RegisterModOptions<Config>();
#else
    [BepInPlugin("EldritchCarMaker.CyclopsVehicleUpgradeConsole", "Cyclops Vehicle Upgrade Console", "1.1.1")]
    public class QMod : BaseUnityPlugin
    {
        internal static Config config { get; } = OptionsPanelHandler.RegisterModOptions<Config>();
#endif
#if !SN2
        [QModPatch]
        public static void Patch()
        {
#else
        public void Awake()
        {
#endif
            var assembly = Assembly.GetExecutingAssembly();
            var name = ($"EldritchCarMaker_{assembly.GetName().Name}");
#if !SN2
            Logger.Log(Logger.Level.Info, $"Patching {name}");
#else
            Logger.LogInfo($"Patching {name}");
#endif
            Harmony harmony = new Harmony(name);
            harmony.PatchAll(assembly);
#if !SN2
            Logger.Log(Logger.Level.Info, "Patched successfully!");
#else
            Logger.LogInfo("Patched successfully!");
#endif
        }
    }
    [Menu("Cyclops Vehicle Upgrade Console")]
    public class Config : ConfigFile
    {
        [Toggle("Hide Unknown Vehicles", Tooltip = "Hides the vehicle creation button if you haven't unlocked the vehicle yet")]
        public bool hideUnknown = false;
    }
}