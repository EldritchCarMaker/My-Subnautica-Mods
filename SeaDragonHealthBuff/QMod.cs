using System.Reflection;
using HarmonyLib;
using QModManager.API.ModLoading;
using Logger = QModManager.Utility.Logger;
using SMLHelper.V2.Json;
using SMLHelper.V2.Options.Attributes;
using SMLHelper.V2.Handlers;

namespace AdaptiveTeleportingCosts
{
    [QModCore]
    public static class QMod
    {
        internal static Config config { get; } = OptionsPanelHandler.Main.RegisterModOptions<Config>();
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
    [Menu("Adaptive Teleporting Costs")]
    public class Config : ConfigFile
    {
        public float minimumTeleportCost = 50;
        
        public float maximumTeleportCost = 2000;

        [Slider("Cost per Distance", Format = "{0:F2}", Min = 0.1F, Max = 5f, DefaultValue = 2.0F, Step = 0.5F, Tooltip = "How much energy per meter is used when teleporting. Defaults to 2 energy per meter")]
        public float distanceCostMultiplier = 2.0F;

        [Toggle("Stop FCS PDA voicelines", Tooltip = "Toggles whether or not this mod stops the FCS PDA from playing those loud and annoying voice lines every time its first used")]
        public bool stopPDAVoice = false;
    }
}