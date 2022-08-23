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
using EquivalentExchange.Constructables;

namespace EquivalentExchange
{
    [QModCore]
    public static class QMod
    {
        internal static Config config { get; } = OptionsPanelHandler.Main.RegisterModOptions<Config>();
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

            ConsoleCommandsHandler.Main.RegisterConsoleCommand("ExchangeUnlockAll", typeof (QMod), nameof(ExchangeUnlockAll));
            ConsoleCommandsHandler.Main.RegisterConsoleCommand("ExchangeLockAll", typeof(QMod), nameof(ExchangeLockAll));

            ConsoleCommandsHandler.Main.RegisterConsoleCommand("AddEMC", typeof(QMod), nameof(AddAmount));

            new ItemResearchStationConstructable().Patch();

            Logger.Log(Logger.Level.Info, "Patched successfully!");
        }
        public static void ExchangeUnlockAll()
        {
            ErrorMessage.AddMessage("Unlocked all techtypes for exchange");
            SaveData.learntTechTypes.AddRange(KnownTech.GetAllUnlockables());
        }
        public static void ExchangeLockAll()
        {
            ErrorMessage.AddMessage("Locked all techtypes for exchange");
            SaveData.learntTechTypes.RemoveRange(0, SaveData.learntTechTypes.Count);
        }
        public static void UnlockExchangeType(string str)
        {
            TechType type = GetTechType(str);
            if (type == TechType.None) return;
            if(!SaveData.learntTechTypes.Contains(type))
                SaveData.learntTechTypes.Add(type);
            ErrorMessage.AddMessage($"Unlocked {type}");
        }
        public static void LockExchangeType(string str)
        {
            TechType type = GetTechType(str);
            if (type == TechType.None) return;
            if (SaveData.learntTechTypes.Contains(type))
                SaveData.learntTechTypes.Remove(type);
            ErrorMessage.AddMessage($"Locked {type}");
        }
        public static TechType GetTechType(string value)
        {
            return GetTechType(value, out _);
        }
        public static TechType GetTechType(string value, out bool isModded)
        {
            isModded = false;

            if (string.IsNullOrEmpty(value))
                return TechType.None;

            // Look for a known TechType
            if (TechTypeExtensions.FromString(value, out TechType tType, true))
                return tType;

            isModded = true;
            //  Not one of the known TechTypes - is it registered with SMLHelper?
            if (TechTypeHandler.TryGetModdedTechType(value, out TechType custom))
                return custom;

            return TechType.None;
        }

        public static void AddAmount(int amount) => SaveData.EMCAvailable += amount;
    }
    [Menu("Equivalent Exchange")]
    public class Config : ConfigFile
    {
        [Keybind("Menu Key 1", Tooltip = "Press both this key and Menu Key 2 at the same time to open the exchange menu")]
        public KeyCode menuKey = KeyCode.K; 
        [Keybind("Menu Key 2", Tooltip = "Press both this key and Menu Key 1 at the same time to open the exchange menu")]
        public KeyCode menuKey2 = KeyCode.J;

        public float inefficiencyMultiplier = 1f;

        public Dictionary<TechType, int> BaseMaterialCosts = new Dictionary<TechType, int>()//if you're a modder trying to change this value for your item, please use the ExternalModCompat class
        {
            { TechType.Titanium, 5 },
            { TechType.Copper, 7 },
            { TechType.Sulphur, 20 },
            { TechType.Diamond, 25 },
            { TechType.Gold, 20 },
            { TechType.Kyanite, 75 },
            { TechType.PrecursorIonCrystal, 100 },
            { TechType.Lead, 7 },
            { TechType.Lithium, 20 },
            { TechType.Magnetite, 20 },
            { TechType.ScrapMetal, 20 },
            { TechType.Nickel, 30 },
            { TechType.Quartz, 10 },
            { TechType.AluminumOxide, 30 },
            { TechType.Salt, 5 },
            { TechType.Silver, 10 },
            { TechType.UraniniteCrystal, 25 },
        };
        public Dictionary<TechType, int> OrganicMaterialsCosts = new Dictionary<TechType, int>()//if you're a modder trying to change this value for your item, please use the ExternalModCompat class
        {
            { TechType.CrashPowder, 1 },
            { TechType.AcidMushroom, 7 },
            { TechType.KooshChunk, 15 },
            { TechType.CoralChunk, 15 },
            { TechType.CreepvinePiece, 10 },
            { TechType.CreepvineSeedCluster, 15 },
            { TechType.WhiteMushroom, 25 },
            { TechType.EyesPlantSeed, 15 },
            { TechType.TreeMushroomPiece, 15 },
            { TechType.JellyPlant, 25 },
            { TechType.RedGreenTentacleSeed, 15 },
            { TechType.SeaCrownSeed, 15 },
            { TechType.StalkerTooth, 15 },
            { TechType.JeweledDiskPiece, 10 },
        };
    }
    [FileName("EquivalentExchange")]
    public class SaveData : SaveDataCache
    {
        //public List<string> learntTechTypes = new List<string>();
        public List<TechType> learntTechTypes = new List<TechType>();
        public float EMCAvailable = 0;
    }
}