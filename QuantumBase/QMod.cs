using System.Reflection;
using HarmonyLib;
using QModManager.API.ModLoading;
using Logger = QModManager.Utility.Logger;
using UnityEngine;
using System.Collections.Generic;
using QuantumBase.Items;
using SMLHelper.V2.Crafting;
using SMLHelper.V2.Json;
using SMLHelper.V2.Handlers;
//using MoreCyclopsUpgrades.API;

namespace QuantumBase
{
    [QModCore]
    public static class QMod
    {
        public static SaveData SaveData { get; } = SaveDataHandler.Main.RegisterSaveDataCache<SaveData>();

        public static readonly TechType substanceUnlockType = TechTypeHandler.AddTechType("mysterioussubstanceunlocktype", "Mysterious Substances", "y tho?", false);
        [QModPatch]
        public static void Patch()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var name = ($"EldritchCarMaker_{assembly.GetName().Name}");
            Logger.Log(Logger.Level.Info, $"Patching {name}");
            Harmony harmony = new Harmony(name);
            harmony.PatchAll(assembly);

            PatchSubstances();

            Logger.Log(Logger.Level.Info, "Patched successfully!");
        }
        public static void PatchSubstances()
        {
            var recipe1 = new TechData() 
            { 
                craftAmount = 1,
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient(TechType.Peeper, 2),
                    new Ingredient(TechType.CreepvinePiece, 1)
                }
            };
            new MysteriousSubstance(500, 1, recipe1).Patch();


            var recipe2 = new TechData()
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient(TechType.Reginald, 2),
                    new Ingredient(TechType.HangingFruit, 1)
                }
            };
            new MysteriousSubstance(1000, 2, recipe2).Patch();


            var recipe3 = new TechData()
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient(TechType.Oculus, 2),
                    new Ingredient(TechType.SeaCrownSeed, 1)
                }
            };
            new MysteriousSubstance(2000, 3, recipe3, "Clearly a much higher quality. Now almost looks edible, albeit not tasty.").Patch();
        }
    }
    public class SaveData : SaveDataCache
    {
        public float Energy;
        public bool isInBase;
        public bool wasInLifepod;
        public Vector3 LastPosition;
        public string LastSubRoot;
        public string LastVehicle;
    }
}