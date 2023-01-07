using System.Reflection;
using HarmonyLib;
#if !SN2
using QModManager.API.ModLoading;
using Logger = QModManager.Utility.Logger;
#else
using BepInEx;
using BepInEx.Logging;
#endif
using UnityEngine;
using SMLHelper.V2.Json;
using SMLHelper.V2.Options.Attributes;
using SMLHelper.V2.Handlers;
using System.IO;
using SMLHelper.V2.Json.Attributes;
using System.Collections.Generic;

namespace AutoStorageTransfer
{
#if !SN2
    [QModCore]
    public static class QMod
    {
#else
    [BepInPlugin("EldritchCarMaker.AutoStorageTransfer", "Auto Storage Transfer", "1.0.2")]
    public class QMod : BaseUnityPlugin
    {
#endif
        internal static Config config { get; } = OptionsPanelHandler.Main.RegisterModOptions<Config>();
        internal static SaveData SaveData { get; } = SaveDataHandler.Main.RegisterSaveDataCache<SaveData>();
#if !SN2
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
#if !SN2
            Logger.Log(Logger.Level.Info, $"Patching {name}");
#else
            Logger.LogInfo($"Patching {name}");
#endif
            Harmony harmony = new Harmony(name);
            harmony.PatchAll(assembly);

            new Items.StorageTransferController().Patch();
            Patches.FCSCompatPatches.PatchFCS(harmony);
#if !SN2
            Logger.Log(Logger.Level.Info, "Patched successfully!");
#else
            logger = Logger;
            Logger.LogInfo("Patched successfully!");
#endif
        }
    }
    [FileName("AutoStorageTransfer")]
    public class SaveData : SaveDataCache
    {
        public Dictionary<string, SaveInfo> SavedStorages = new Dictionary<string, SaveInfo>();
    }
    public class SaveInfo
    {
        public string StorageID;
        public bool IsReciever;
    }
    public class Config : ConfigFile
    {
        public float thoroughSortCooldown = 30;
        public int itemChecksBeforeBreak = 10;
    }
}
