using System.Reflection;
using HarmonyLib;
using QModManager.API.ModLoading;
using Logger = QModManager.Utility.Logger;

using SMLHelper.V2.Json;
using SMLHelper.V2.Options.Attributes;
using SMLHelper.V2.Handlers;
using UnityEngine;
using System.Collections.Generic;
using SMLHelper.V2.Json.Attributes;

namespace EquivalentExchange
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
            var stingers = ($"Nagorogan_{assembly.GetName().Name}");
            Logger.Log(Logger.Level.Info, $"Patching {stingers}");
            Harmony harmony = new Harmony(stingers);
            harmony.PatchAll(assembly);

            ConsoleCommandsHandler.Main.RegisterConsoleCommand("UnlockExchangeType", typeof(QMod), nameof(UnlockExchangeType));
            ConsoleCommandsHandler.Main.RegisterConsoleCommand("lockExchangeType", typeof(QMod), nameof(LockExchangeType));

            ConsoleCommandsHandler.Main.RegisterConsoleCommand("AddEMC", typeof(QMod), nameof(AddAmount));

            Logger.Log(Logger.Level.Info, "Patched successfully!");
        }

        public static void UnlockExchangeType(string str)
        {
            TechType type = GetTechType(str);
            if (type == TechType.None) return;
            SaveData.learntTechTypes.Add(type);
        }
        public static void LockExchangeType(string str)
        {
            TechType type = GetTechType(str);
            if (type == TechType.None) return;
            SaveData.learntTechTypes.Remove(type);
        }
        public static TechType GetTechType(string value)
        {
            if (string.IsNullOrEmpty(value))
                return TechType.None;

            // Look for a known TechType
            if (TechTypeExtensions.FromString(value, out TechType tType, true))
                return tType;

            //  Not one of the known TechTypes - is it registered with SMLHelper?
            if (TechTypeHandler.TryGetModdedTechType(value, out TechType custom))
                return custom;

            return TechType.None;
        }
        public static void AddAmount(int amount) => SaveData.EMCAvailable += amount;
    }
    /*[Menu("Cyclops Torpedoes")]
    public class Config : ConfigFile
    {
        [Keybind("Torpedo Key", Tooltip = "Press this key while you are controlling the cyclops cameras in order to shoot a torpedo from the cyclops' decoy tube")]
        public KeyCode torpedoKey = KeyCode.F;
        public TechType priorityTorpedoType = TechType.GasTorpedo;
        public Dictionary<string, int> torpedoTypePriority = new Dictionary<string, int>();
    }*/
    [FileName("EquivalentExchange")]
    public class SaveData : SaveDataCache
    {
        //public List<string> learntTechTypes = new List<string>();
        public List<TechType> learntTechTypes = new List<TechType>();
        public int EMCAvailable = 0;
    }
}