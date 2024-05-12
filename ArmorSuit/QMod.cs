using System.Reflection;
using HarmonyLib;
#if !SN2
using QModManager.API.ModLoading;
using Logger = QModManager.Utility.Logger;
using SMLHelper.V2.Json;
using SMLHelper.V2.Options.Attributes;
using SMLHelper.V2.Handlers;
#else
using BepInEx;
using BepInEx.Logging;
using Nautilus.Handlers;
using Nautilus.Json;
using Nautilus.Options.Attributes;
#endif
using UnityEngine;
using System.IO;
using ArmorSuit.Items;
using System.Collections.Generic;
using static ArmorSuit.ArmorSuitMono;
using Nautilus.Utility;
using SuitLib;

namespace ArmorSuit
{
#if !SN2
    [QModCore]
    public static class QMod
    {
        public static Config config { get; } = OptionsPanelHandler.Main.RegisterModOptions<Config>();
#else
    [BepInPlugin("EldritchCarMaker.ArmorSuit", "Armor Suit", "1.0.3")]
    [BepInDependency("com.snmodding.nautilus", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("Indigocoder.SuitLib", BepInDependency.DependencyFlags.HardDependency)]
    public class QMod : BaseUnityPlugin
    {
        public static Config config { get; } = OptionsPanelHandler.RegisterModOptions<Config>();
#endif
#if !SN2
        [QModPatch]
        public static void Patch()
#else
        public void Awake()
#endif
        {
            var assembly = Assembly.GetExecutingAssembly();
            var CyclopsLockers = ($"EldritchCarMaker_{assembly.GetName().Name}");
#if !SN2
            Logger.Log(Logger.Level.Info, $"Patching {CyclopsLockers}");
#else
            Logger.LogInfo($"Patching {CyclopsLockers}");
#endif
            Harmony harmony = new Harmony(CyclopsLockers);
            harmony.PatchAll(assembly);

            IonFiber.Patch();
            ArmorGlovesItem.Patch();
            ArmorSuitItem.Patch();
            RegisterSuit();

#if !SN2
            Logger.Log(Logger.Level.Info, "Patched successfully!");
#else
            Logger.LogInfo("Patched successfully!");
#endif
        }
        private void RegisterSuit()//Massive thanks to GamingForFun/IndigoCoder for porting it to their SuitLibrary. Wouldn't have done it without them
        {
            Texture2D bodyTex = ImageUtils.LoadTextureFromFile(Path.Combine(AssetsFolder, "ColoredSuitBody.png"));
            Texture2D armsTex = ImageUtils.LoadTextureFromFile(Path.Combine(AssetsFolder, "ColoredSuitArms.png"));

            //Inspiring naming, I know
            Dictionary<string, Texture2D> suitKeyValuePairs = new Dictionary<string, Texture2D> { { "_MainTex", bodyTex }, { "_SpecTex", bodyTex } };
            Dictionary<string, Texture2D> armKeyValuePairs = new Dictionary<string, Texture2D> { { "_MainTex", armsTex }, { "_SpecTex", armsTex } };
            ModdedSuit armorSuit = new(suitKeyValuePairs, armKeyValuePairs, ModdedSuitsManager.VanillaModel.Reinforced, ArmorSuitItem.thisTechType, tempValue: 0);
            ModdedGloves armorGloves = new(armKeyValuePairs, ModdedSuitsManager.VanillaModel.Reinforced, ArmorGlovesItem.techType, tempValue: 0);

            ModdedSuitsManager.AddModdedSuit(armorSuit);
            ModdedSuitsManager.AddModdedGloves(armorGloves);
        }
    }
    [Menu("ArmorSuit")]
    public class Config : ConfigFile
    {
        [Keybind("Armor Suit Key", Tooltip = "Press this key while you have the the armor suit equipped to switch the current damage type")]
        public KeyCode ArmorSuitKey = KeyCode.X;
        [Toggle("Automatic", Tooltip = "Toggles whether the armor suit will automatically adapt to the last damage type taken, or if it will simply stay as the type specified")]
        public bool Automatic = true;

        public List<ArmorSuitMono.DefenseInfo> DefenseInfos = new List<ArmorSuitMono.DefenseInfo>()
        {
            new DefenseInfo(
                    DefenseType.Physical,
                    new Color(0.6f, 0.6f, 0.6f),
                    new List<DamageType>()
                    {
                        DamageType.Normal,
                        DamageType.Collide,
                        DamageType.Puncture,
                        DamageType.Drill
                    }
               ),
            new DefenseInfo(
                    DefenseType.Electrical,
                    new Color(0, 0.235f, 1f),
                    new List<DamageType>()
                    {
                        DamageType.Electrical
                    }
               ),
            new DefenseInfo(
                    DefenseType.Thermal,
                    new Color(1, 0.314f, 0),
                    new List<DamageType>()
                    {
                        DamageType.Heat,
                        DamageType.Fire
                    }
               ),
            new DefenseInfo(
                    DefenseType.Acidic,
                    new Color(0, 0.75f, 0),
                    new List<DamageType>()
                    {
                        DamageType.Acid
                    }
               ),
            new DefenseInfo(
                    DefenseType.Poisonous,
                    new Color(0, 1, 0),
                    new List<DamageType>()
                    {
                        DamageType.Poison,
                    }
               ),
            new DefenseInfo(
                    DefenseType.Cold,
                    new Color(0, 0.725f, 1),
                    new List<DamageType>()
                    {
                        DamageType.Cold,
                    }
               ),
            new DefenseInfo(
                    DefenseType.Radioactive,
                    new Color(1, 1, 0),
                    new List<DamageType>()
                    {
                        DamageType.Radiation,
                    }
               ),
            new DefenseInfo(
                    DefenseType.Explosive,
                    new Color(1, 0, 0),
                    new List<DamageType>()
                    {
                        DamageType.Explosive,
                    }
               )
        };
    }
}
