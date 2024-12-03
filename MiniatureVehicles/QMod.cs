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
#endif
using UnityEngine;

namespace MiniatureVehicles
{
#if !SN2
    [QModCore]
    public static class QMod
    {
#else
    [BepInPlugin("EldritchCarMaker.MiniatureVehicles", "Miniature Vehicles", "1.0.1")]
    public class QMod : BaseUnityPlugin
    {
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
            new MiniatureVehicleModule().Patch();
#if !SN2
            Logger.Log(Logger.Level.Info, "Patched successfully!");
#else
            Logger.LogInfo("Patched successfully!");
#endif
        }
    }
}
