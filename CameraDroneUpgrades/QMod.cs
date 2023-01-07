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
using System.Collections.Generic;
using SMLHelper.V2.Assets;
using CameraDroneUpgrades.API;

namespace CameraDroneUpgrades
{
#if !SN2
    [QModCore]
    public static class QMod
    {
#else
    [BepInPlugin("EldritchCarMaker.CameraDroneUpgrades", "Camera Drone Upgrades", "1.1.0")]
    public class QMod : BaseUnityPlugin
    {
#endif
        private static Assembly assembly = Assembly.GetExecutingAssembly();
        private static string modPath = Path.GetDirectoryName(assembly.Location);

        internal static Config Config { get; } = OptionsPanelHandler.Main.RegisterModOptions<Config>();
#if !SN2
        [QModPatch]
        public static void Patch()
        {
#else
        public static ManualLogSource logger;
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

            CraftTreeHandler.AddTabNode(CraftTree.Type.MapRoom, Registrations.upgradeModulePaths[0], "Drone Upgrades", SpriteManager.Get(TechType.MapRoomCamera));

#if !SN2
            Logger.Log(Logger.Level.Info, "Patched successfully!");
#else
            logger = Logger;
            Logger.LogInfo("Patched successfully!");
#endif
        }
    }
    [Menu("Camera Drone Upgrades")]
    public class Config : ConfigFile
    {
        [Keybind("Scan Key", Tooltip = "When using a camera drone, hold this key to begin scanning an object")]
        public KeyCode scanKey = KeyCode.F;
    }
}
