using System.Reflection;
using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;
using MoreCyclopsUpgrades.API;
using CyclopsTorpedoes.Patches;
using System.Collections;

#if SN1
using QModManager.API.ModLoading;
using Logger = QModManager.Utility.Logger;

using SMLHelper.V2.Json;
using SMLHelper.V2.Options.Attributes;
using SMLHelper.V2.Handlers;
#else
using BepInEx;
using Nautilus.Json;
using Nautilus.Options.Attributes;
using Nautilus.Handlers;
#endif

namespace CyclopsTorpedoes;

#if SN1
[QModCore]
public static class QMod
{
    internal static Config config { get; } = OptionsPanelHandler.Main.RegisterModOptions<Config>();
    [QModPatch]
    public static void Patch()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var stingers = ($"EldritchCarMaker_{assembly.GetName().Name}");
        Logger.Log(Logger.Level.Info, $"Patching {stingers}");
        Harmony harmony = new Harmony(stingers);
        harmony.PatchAll(assembly);

        var module = new TorpedoModule();
        module.Patch();

        MCUServices.Register.CyclopsUpgradeHandler((SubRoot cyclops) =>
        {
            return new MoreCyclopsUpgrades.API.Upgrades.UpgradeHandler(module.TechType, cyclops);
        });
        UWE.CoroutineHost.StartCoroutine(CyclopsExternalCamsPatches.RefreshTorpedoTypes());

        Logger.Log(Logger.Level.Info, "Patched successfully!");
    }
}
#else
[BepInPlugin("EldritchCarMaker.CyclopsTorpedoes", "Cyclops Torpedoes", "1.0.0")]
public class QMod : BaseUnityPlugin
{
    internal static Config config { get; } = OptionsPanelHandler.RegisterModOptions<Config>();
    public void Awake()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var stingers = ($"EldritchCarMaker_{assembly.GetName().Name}");
        Logger.LogInfo($"Patching {stingers}");
        Harmony harmony = new Harmony(stingers);
        harmony.PatchAll(assembly);

        var module = new TorpedoModule();
        module.Info.WithIcon(module.GetItemSprite());//Purple come on now
        module.Patch();

        MCUServices.Register.CyclopsUpgradeHandler((SubRoot cyclops) =>
        {
            return new MoreCyclopsUpgrades.API.Upgrades.UpgradeHandler(module.TechType, cyclops);
        });

        Logger.LogInfo("Patched successfully!");
    }
    private IEnumerator Start()
    {
        yield return CyclopsExternalCamsPatches.RefreshTorpedoTypes();
    }
}
#endif
[Menu("Cyclops Torpedoes")]
public class Config : ConfigFile
{
    [Keybind("Torpedo Key", Tooltip = "Press this key while you are controlling the cyclops cameras in order to shoot a torpedo from the cyclops' decoy tube")]
    public KeyCode torpedoKey = KeyCode.F;
    public TechType priorityTorpedoType = TechType.GasTorpedo;
    public Dictionary<string, int> torpedoTypePriority = new Dictionary<string, int>();
}