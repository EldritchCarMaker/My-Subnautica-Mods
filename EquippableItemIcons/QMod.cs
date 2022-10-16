using System.Reflection;
using HarmonyLib;
using QModManager.API.ModLoading;
using Logger = QModManager.Utility.Logger;
using UnityEngine;
using SMLHelper.V2.Json;
using SMLHelper.V2.Options.Attributes;
using SMLHelper.V2.Handlers;
using System.IO;

namespace EquippableItemIcons
{
    [QModCore]
    public static class QMod
    {
        public static Config config { get; } = OptionsPanelHandler.Main.RegisterModOptions<Config>();
        [QModPatch]
        public static void Patch()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var CyclopsLockers = ($"EldritchCarMaker_{assembly.GetName().Name}");
            Logger.Log(Logger.Level.Info, $"Patching {CyclopsLockers}");
            Harmony harmony = new Harmony(CyclopsLockers);
            harmony.PatchAll(assembly);

            Logger.Log(Logger.Level.Info, "Patched successfully!");
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
