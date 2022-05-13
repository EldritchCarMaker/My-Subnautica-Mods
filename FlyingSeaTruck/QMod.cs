using System.Reflection;
using HarmonyLib;
using QModManager.API.ModLoading;
using Logger = QModManager.Utility.Logger;

using SMLHelper.V2.Json;
using SMLHelper.V2.Options.Attributes;
using SMLHelper.V2.Handlers;

namespace FlyingSeaTruck
{
    [QModCore]
    public static class QMod
    {
        public static Config config { get; } = OptionsPanelHandler.Main.RegisterModOptions<Config>();
        [QModPatch]
        public static void Patch()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var CyclopsLockers = ($"Nagorogan_{assembly.GetName().Name}");
            Logger.Log(Logger.Level.Info, $"Patching {CyclopsLockers}");
            Harmony harmony = new Harmony(CyclopsLockers);
            harmony.PatchAll(assembly);

            new SeatruckFlightModule().Patch();

            Logger.Log(Logger.Level.Info, "Patched successfully!");
        }
    }
    [Menu("Flying SeaTruck")]
    public class Config : ConfigFile
    {
        [Toggle("SeaTruck falls when not piloted", Tooltip = "Toggles whether the SeaTruck will fall to the ground when not being piloted or continue to float")]
        public bool SeaTruckFalls = true;
    }
}
