using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
#if SN1
using Logger = QModManager.Utility.Logger;
using QModManager.API.ModLoading;
using SMLHelper.V2.Handlers;
using SMLHelper.V2.Json;
using SMLHelper.V2.Json.Attributes;
#else
using BepInEx;
using Nautilus.Handlers;
using Nautilus.Json;
using Nautilus.Json.Attributes;
#endif

namespace EdibleEverything
{
#if !SN2
    [QModCore]
    public static class QMod
    {
#else
    [BepInPlugin("EldritchCarMaker.EdibleEverything", "Edible Everything", "1.0.2")]
    public class QMod : BaseUnityPlugin
    {
#endif
#if !SN2
        internal static SaveData SaveData { get; } = SaveDataHandler.Main.RegisterSaveDataCache<SaveData>();
        [QModPatch]
        public static void Patch()
        {
#else
        internal static SaveData SaveData { get; } = SaveDataHandler.RegisterSaveDataCache<SaveData>();
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
#if !SN2
            Logger.Log(Logger.Level.Info, "Patched successfully!");
#else
            Logger.LogInfo("Patched successfully!");
#endif
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
#if BZ
        public int Charges;
        public int Health;
        public int Heat;
#endif
    }
}
