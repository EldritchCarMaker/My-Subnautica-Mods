using System.Reflection;
using HarmonyLib;
#if SN1
using QModManager.API.ModLoading;
using Logger = QModManager.Utility.Logger;
using SMLHelper.V2.Json;
using SMLHelper.V2.Options.Attributes;
using SMLHelper.V2.Handlers;
using SMLHelper.V2.Json.Attributes;
#else
using BepInEx;
using BepInEx.Logging;
using Nautilus.Handlers;
using Nautilus.Json;
#endif
using AutoStorageTransfer.Json;

namespace AutoStorageTransfer
{
#if SN1
    [QModCore]
    public static class QMod
    {
#else
    [BepInPlugin("EldritchCarMaker.AutoStorageTransfer", "Auto Storage Transfer", "1.0.3")]
    public class QMod : BaseUnityPlugin
    {
#endif
        internal static Config config { get; } = OptionsPanelHandler.Main.RegisterModOptions<Config>();
        internal static SaveData SaveData { get; } = SaveDataHandler.Main.RegisterSaveDataCache<SaveData>();
#if SN1
        [QModPatch]
        public static void Patch()
        {
#else
        internal static ManualLogSource logger;
        public void Awake()
        {
#endif
            var assembly = Assembly.GetExecutingAssembly();
            var name = ($"EldritchCarMaker_{assembly.GetName().Name}");
#if SN1
            Logger.Log(Logger.Level.Info, $"Patching {name}");
#else
            Logger.LogInfo($"Patching {name}");
#endif
            Harmony harmony = new Harmony(name);
            harmony.PatchAll(assembly);

            new Items.StorageTransferController().Patch();
            Patches.FCSCompatPatches.PatchFCS(harmony);
#if SN1
            Logger.Log(Logger.Level.Info, "Patched successfully!");
#else
            logger = Logger;
            Logger.LogInfo("Patched successfully!");
#endif
        }
    }
    public class Config : ConfigFile
    {
        public float thoroughSortCooldown = 30;
        public int itemChecksBeforeBreak = 10;
    }
}
