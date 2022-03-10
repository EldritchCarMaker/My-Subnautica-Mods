using System.Reflection;
using HarmonyLib;
using QModManager.API.ModLoading;
using Logger = QModManager.Utility.Logger;

using SMLHelper.V2.Json;
using SMLHelper.V2.Options.Attributes;
using SMLHelper.V2.Handlers;

namespace BetterCyclopsLockers
{
    [QModCore]
    public static class QMod
    {
        internal static Config Config { get; } = OptionsPanelHandler.Main.RegisterModOptions<Config>();
        [QModPatch]
        public static void Patch()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var CyclopsLockers = ($"Nagorogan_{assembly.GetName().Name}");
            Logger.Log(Logger.Level.Info, $"Patching {CyclopsLockers}");
            Harmony harmony = new Harmony(CyclopsLockers);
            harmony.PatchAll(assembly);
            Logger.Log(Logger.Level.Info, "Patched successfully!");
        }
    }
    [Menu("Better Cyclops Lockers")]
    public class Config : ConfigFile
    {   
		/* Do something with this eventually
		 * private static bool bHasAdvancedInventory => QModManager.API.QModServices.Main.ModPresent(AdvancedInventoryAssembly);
		 * private static int MaxHeight => (bHasAdvancedInventory ? 100 : 15);
        */
        [Slider("Cyclops locker height", Format = "{0:F2}", Min = 1.0F, Max = 100.0f, DefaultValue = 7.0F, Step = 1.0F, Tooltip = "Warning, going above 10 can cause slots to begin to overlap and/or other issues to appear.   Must reload save to take effect.")]
        public float Height = 7.0F;
        [Slider("Cyclops locker width", Format = "{0:F2}", Min = 1.0F, Max = 100.0F, DefaultValue = 7.0F, Step = 1.0F, Tooltip = "Warning, going above 10 can cause slots to begin to overlap and/or other issues to appear.   Must reload save to take effect.")]
        public float Width = 7.0F;
    }
}
