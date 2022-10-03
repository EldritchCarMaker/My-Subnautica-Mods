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
        //internal static Config config { get; } = OptionsPanelHandler.Main.RegisterModOptions<Config>();
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
    /*
    [Menu("No FCS Drone Port")]
    public class Config : ConfigFile
    {
        [Toggle("Need Depot", Tooltip = "Toggles whether you need a depot in base, or if items are simply delivered directly to your inventory")]
        public bool needDepo = true;
        [Toggle("Mimic Drone Travel", Tooltip = "Toggles whether items will be put on a timer to mimic how long it takes for the drone to travel to the depot, without actually having the drone travel")]
        public bool mimicDroneTravel = false;
        [Toggle("Mimic Travel Time Message", Tooltip = "When Mimic Drone Travel is true, this toggles whether or not there will be a message on screen for when items arrive in the depot")]
        public bool sendMessage = true;
        public int minRandomTimeForMimic = 30;don't feel like doing this right now
        public int maxRandomTimeForMimic = 60;
    }*/
}
