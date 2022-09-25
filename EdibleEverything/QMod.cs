using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using QModManager.API.ModLoading;
using SMLHelper.V2.Json;
using SMLHelper.V2.Json.Attributes;
using Logger = QModManager.Utility.Logger;

namespace EdibleEverything
{
    [QModCore]
    public static class QMod
    {
        public static SaveData SaveData { get; } = SMLHelper.V2.Handlers.SaveDataHandler.Main.RegisterSaveDataCache<SaveData>();
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
    [FileName("EdibleEverything")]
    public class SaveData : SaveDataCache
    {
        public Dictionary<TechType, FoodValues> foodValues = new Dictionary<TechType, FoodValues>();
    }
    public class FoodValues
    {
        public int Food;
        public int Water;
    }
}
