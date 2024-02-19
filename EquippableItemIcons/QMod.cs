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
using Nautilus.Handlers;
using Nautilus.Json;
using Nautilus.Options.Attributes;
#endif
using UnityEngine;
using System.IO;

namespace EquippableItemIcons
{
#if !SN2
    [QModCore]
    public static class QMod
    {
    
        internal static Config config { get; } = OptionsPanelHandler.Main.RegisterModOptions<Config>();
#else
    [BepInPlugin("EldritchCarMaker.EquippableItemIcons", "Equippable Item Icons", "1.4.0")]
    public class QMod : BaseUnityPlugin
    {
        internal static Config config { get; } = OptionsPanelHandler.RegisterModOptions<Config>();
#endif
#if !SN2
        [QModPatch]
        public static void Patch()
        {
#else
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
            Logger.LogMessage("Patched Successfully!");
#endif
        }
    }
    [Menu("Equippable Item Icons")]
    public class Config : ConfigFile
    {
        [Toggle("Sounds", Tooltip = "Toggles whether sounds are played through this mod. Be aware, other mods may still play sounds on their on accord, this can not change those")]
        public bool SoundsActive = true;
        public float iconSizeScale = 1;
    }
}
