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
using RechargerChips.Items;

namespace RechargerChips
{
#if !SN2
    [QModCore]
    public static class QMod
    {
#else
    [BepInPlugin("EldritchCarMaker.RechargerChips", "Recharger Chips", "1.0.2")]
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

#if SN1
            new SolarChargerChip().Patch();
            new ThermalChargerChip().Patch();
            new ComboChargerChip().Patch();
#else
            ChargerChipItem.Patch();
#endif

#if !SN2
            Logger.Log(Logger.Level.Info, "Patched successfully!");
#else
            Logger.LogInfo("Patched successfully!");
#endif
        }
    }
}
