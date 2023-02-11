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
using ArmorSuit.Items;
using System.Collections.Generic;
using static ArmorSuit.ArmorSuitMono;

namespace ArmorSuit
{
#if !SN2
    [QModCore]
    public static class QMod
#else
    [BepInPlugin("EldritchCarMaker.ArmorSuit", "Armor Suit", "1.0.2")]
    public class QMod : BaseUnityPlugin
#endif
    {
        public static Config config { get; } = OptionsPanelHandler.Main.RegisterModOptions<Config>();
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

            new IonFiber().Patch();
            new ArmorGlovesItem().Patch();
            new ArmorSuitItem().Patch();

#if !SN2
            Logger.Log(Logger.Level.Info, "Patched successfully!");
#else
            Logger.LogInfo("Patched successfully!");
#endif
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
