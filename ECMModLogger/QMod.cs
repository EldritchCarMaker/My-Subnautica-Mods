using System.Reflection;
using HarmonyLib;
using QModManager.API.ModLoading;
using Logger = QModManager.Utility.Logger;
using UnityEngine;
using System.Collections.Generic;
using SMLHelper.V2.Json.Attributes;
using SMLHelper.V2.Json;
using SMLHelper.V2.Handlers;

namespace CyclopsTorpedoes
{
    [QModCore]
    public static class QMod
    {
        internal static SaveData Save { get; } = SaveDataHandler.Main.RegisterSaveDataCache<SaveData>();
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
    [FileName("ECMModLogger")]
    public class SaveData : SaveDataCache
    {
        public List<string> CollectedCapsules = new List<string>();
    }
}