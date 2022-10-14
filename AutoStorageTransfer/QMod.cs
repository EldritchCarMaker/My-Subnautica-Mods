using System.Reflection;
using HarmonyLib;
using QModManager.API.ModLoading;
using Logger = QModManager.Utility.Logger;
using UnityEngine;
using SMLHelper.V2.Json;
using SMLHelper.V2.Options.Attributes;
using SMLHelper.V2.Handlers;
using System.IO;
using SMLHelper.V2.Json.Attributes;
using System.Collections.Generic;

namespace AutoStorageTransfer
{
    [QModCore]
    public static class QMod
    {
        public static Config config { get; } = OptionsPanelHandler.Main.RegisterModOptions<Config>();
        internal static SaveData SaveData { get; } = SaveDataHandler.Main.RegisterSaveDataCache<SaveData>();
        [QModPatch]
        public static void Patch()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var name = ($"Nagorogan_{assembly.GetName().Name}");
            Logger.Log(Logger.Level.Info, $"Patching {name}");
            Harmony harmony = new Harmony(name);
            harmony.PatchAll(assembly);

            new Items.StorageTransferController().Patch();
            Patches.FCSCompatPatches.PatchFCS(harmony);

            Logger.Log(Logger.Level.Info, "Patched successfully!");
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
