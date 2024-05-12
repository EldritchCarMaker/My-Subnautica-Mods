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

namespace MiniatureSuit
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
    [BepInEx.BepInPlugin("EldritchCarMaker.MiniatureSuit", "Miniature Suit", "1.0.0")]
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

            MiniSuitItem.Patch();

            Logger.LogInfo("Patched successfully!");
        }
    }
#endif
    [Menu("Miniature Suit")]
    public class Config : ConfigFile
    {
        [Keybind("Miniature Suit Key", Tooltip = "Press this key while you have a Miniature Suit equipped to use it")]
        public KeyCode MiniSuitKey = KeyCode.F;
    }
}
