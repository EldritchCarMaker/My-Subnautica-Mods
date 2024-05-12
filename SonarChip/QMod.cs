using System.Reflection;
using HarmonyLib;
#if SN1
using QModManager.API.ModLoading;
using Logger = QModManager.Utility.Logger;
using SMLHelper.V2.Json;
using SMLHelper.V2.Options.Attributes;
using SMLHelper.V2.Handlers;
#else
using Nautilus.Json;
using Nautilus.Options.Attributes;
using Nautilus.Handlers;
#endif
using System.IO;
using UnityEngine;
using BepInEx;

namespace SonarChip
{
#if SN1
    [QModCore]
    public static class QMod
    {
        internal static Config config { get; } = OptionsPanelHandler.Main.RegisterModOptions<Config>();
        [QModPatch]


        public static void Patch()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var CyclopsLockers = ($"EldritchCarMaker_{assembly.GetName().Name}");
            Logger.Log(Logger.Level.Info, $"Patching {CyclopsLockers}");
            Harmony harmony = new Harmony(CyclopsLockers);
            harmony.PatchAll(assembly);

            new BurstFinsItem().Patch();

            Logger.Log(Logger.Level.Info, "Patched successfully!");
        }
    }
#else
    [BepInEx.BepInPlugin("EldritchCarMaker.SonarChip", "Sonar Chip", "1.0.0")]
    public class QMod : BaseUnityPlugin
    {
        internal static Config config { get; } = OptionsPanelHandler.RegisterModOptions<Config>();


        public void Awake()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var CyclopsLockers = ($"EldritchCarMaker_{assembly.GetName().Name}");
            Logger.LogInfo($"Patching {CyclopsLockers}");
            Harmony harmony = new Harmony(CyclopsLockers);
            harmony.PatchAll(assembly);

            SonarChipItem.Patch();

            Logger.LogInfo("Patched successfully!");
        }
    }
#endif
    [Menu("Sonar Chip")]
    public class Config : ConfigFile
    {
        [Keybind("Sonar Key", Tooltip = "Press this key while you have a Sonar Chip equipped to let out a sonar ping")]
        public KeyCode ControlKey = KeyCode.J;
    }
}
