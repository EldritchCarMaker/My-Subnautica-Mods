using System.Reflection;
using HarmonyLib;
using QModManager.API.ModLoading;
using Logger = QModManager.Utility.Logger;

using SMLHelper.V2.Json;
using SMLHelper.V2.Options.Attributes;
using SMLHelper.V2.Handlers;
using UnityEngine;

namespace PickupableVehicles
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

            if (config.needsModule) new PickupableVehicleModule().Patch();

            Logger.Log(Logger.Level.Info, "Patched successfully!");
        }
    }
    [Menu("Pickupable Vehicles")]
    public class Config : ConfigFile
    {
        [Toggle("Needs module", Tooltip = "Toggles whether you need an upgrade module equipped to be able to pick up vehicles.")]
        public bool needsModule = false;
        [Toggle("Pickupable Prawn", Tooltip = "Toggles whether the Prawn suit can be picked up. Quite buggy for now, advised to keep off.")]
        public bool worksWithPrawn = false;
#if SN
        [Toggle("Pickupable Cyclops", Tooltip = "Whether or not the cyclops can be picked up. obviously decently buggy, be warned")]
        public bool worksWithCyclops = true;
#else
        [Toggle("Pickupable Seatruck", Tooltip = "Whether or not the seatruck can be picked up. obviously decently buggy, be warned")]
        public bool worksWithSeatruck = true;
#endif
        public int seamothWidth = 2;
        public int seamothHeight = 2;
        public int prawnWidth = 3;
        public int prawnHeight = 3;
#if SN
        public int cyclopsWidth = 5;
        public int cyclopsHeight = 5;
#else
        public int seatruckModuleHeight = 4;
        public int seatruckModuleWidth = 4;
#endif
    }
}
