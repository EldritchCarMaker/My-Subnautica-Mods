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

namespace SpyWatch
{
#if SN1
    [QModCore]
    public static class QMod
    {
        private const string bundlePath = "Assets/SpyWatch.shaders";
        private static Assembly assembly = Assembly.GetExecutingAssembly();
        private static string modPath = Path.GetDirectoryName(assembly.Location);
        internal static AssetBundle assetBundle = AssetBundle.LoadFromFile(Path.Combine(modPath, bundlePath));

        public static Material CloakMaterial;
        internal static Config config { get; } = OptionsPanelHandler.Main.RegisterModOptions<Config>();
        [QModPatch]
        public static void Patch()
        {
            var CyclopsLockers = ($"EldritchCarMaker_{assembly.GetName().Name}");
            Logger.Log(Logger.Level.Info, $"Patching {CyclopsLockers}");
            Harmony harmony = new Harmony(CyclopsLockers);
            harmony.PatchAll(assembly);

            new SpyWatchItem().Patch();

            if(!assetBundle)
            {
                Logger.Log(Logger.Level.Error, "Could not load Asset Bundle");
                return;
            }
            var StealthEffect = assetBundle.LoadAsset<Material>("Cloak_Material_mtl");
            if (!StealthEffect)
            {
                Logger.Log(Logger.Level.Error, "Could not load material from bundle");
                return;
            }
            var StealthEffectShader = assetBundle.LoadAsset<Shader>("Invisibility");
            if (!StealthEffectShader)
            {
                Logger.Log(Logger.Level.Error, "Could not load shader from bundle");
                return;
            }
            StealthEffect.shader = StealthEffectShader;
            assetBundle.Unload(false);
            CloakMaterial = StealthEffect;

            Logger.Log(Logger.Level.Info, "Patched successfully!");
        }
    }
#else
    [BepInEx.BepInPlugin("EldritchCarMaker.SpyWatch", "Spy Watch", "1.0.1")]
    public class QMod : BaseUnityPlugin
    {
        private const string bundlePath = "Assets/SpyWatch.shaders";
        private static Assembly assembly = Assembly.GetExecutingAssembly();
        private static string modPath = Path.GetDirectoryName(assembly.Location);
        internal static AssetBundle assetBundle = AssetBundle.LoadFromFile(Path.Combine(modPath, bundlePath));
        internal static Config config { get; } = OptionsPanelHandler.RegisterModOptions<Config>();


        public static Material CloakMaterial;
        public void Awake()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var CyclopsLockers = ($"EldritchCarMaker_{assembly.GetName().Name}");
            Logger.LogInfo($"Patching {CyclopsLockers}");
            Harmony harmony = new Harmony(CyclopsLockers);
            harmony.PatchAll(assembly);

            SpyWatchItem.Patch();

            if (!assetBundle)
            {
                Logger.LogError("Could not load Asset Bundle");
                return;
            }
            var StealthEffect = assetBundle.LoadAsset<Material>("Cloak_Material_mtl");
            if (!StealthEffect)
            {
                Logger.LogError("Could not load material from bundle");
                return;
            }
            var StealthEffectShader = assetBundle.LoadAsset<Shader>("Invisibility");
            if (!StealthEffectShader)
            {
                Logger.LogError("Could not load shader from bundle");
                return;
            }
            StealthEffect.shader = StealthEffectShader;
            assetBundle.Unload(false);
            CloakMaterial = StealthEffect;

            Logger.LogInfo("Patched successfully!");
        }
    }
#endif
    [Menu("SpyWatch")]
    public class Config : ConfigFile
    {
        [Keybind("Spy Watch Key", Tooltip = "Press this key while you have a Spy Watch equipped to toggle it")]
        public KeyCode SpyWatchKey = KeyCode.Z;
    }
}
