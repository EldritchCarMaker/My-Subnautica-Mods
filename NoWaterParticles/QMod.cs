using System.Reflection;
using HarmonyLib;
#if !SN2
using QModManager.API.ModLoading;
using Logger = QModManager.Utility.Logger;
using SMLHelper.V2.Json;
using SMLHelper.V2.Options.Attributes;
using SMLHelper.V2.Handlers;
using SMLHelper.V2.Options;
#else
using BepInEx;
using Nautilus.Handlers;
using Nautilus.Options.Attributes;
using Nautilus.Json;
using Nautilus.Options;
#endif

namespace NoWaterParticles
{
#if !SN2
    [QModCore]
    public static class QMod
    {
#else
    [BepInPlugin("EldritchCarMaker.NoWaterParticles", "No Water Particles", "1.0.0")]
    public class QMod : BaseUnityPlugin
    {
#endif
        public static Config Config { get; } = OptionsPanelHandler.RegisterModOptions<Config>();
        internal static Assembly assembly = Assembly.GetExecutingAssembly();
        internal static string name = ($"EldritchCarMaker_{assembly.GetName().Name}");
        internal static Harmony harmony { get; } = new Harmony(name);
#if !SN2
        [QModPatch]
        public static void Patch()
        {
#else
        public void Awake()
        {
#endif
#if !SN2
            Logger.Log(Logger.Level.Info, $"Patching {name}");
#else
            Logger.LogInfo($"Patching {name}");
#endif
            if(Config.modEnabled)
                harmony.PatchAll(assembly);
#if !SN2
            Logger.Log(Logger.Level.Info, "Patched successfully!");
#else
            Logger.LogInfo("Patched successfully!");
#endif
        }
    }

    [Menu("No Water Particles")]
    public class Config : ConfigFile
    {
        [Toggle("Mod enabled"), OnChange(nameof(OnConfigChange))]
        public bool modEnabled = true;
        public void OnConfigChange(ToggleChangedEventArgs e)
        {
            if (modEnabled)
                QMod.harmony.PatchAll(QMod.assembly);
            else
                QMod.harmony.UnpatchSelf();
        }
    }
}
