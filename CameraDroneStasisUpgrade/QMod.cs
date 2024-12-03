using System.Reflection;
using HarmonyLib;
using UnityEngine;
using System.IO;
using CameraDroneUpgrades.API;
using BepInEx;
using Nautilus.Handlers;
using Nautilus.Options.Attributes;
using Nautilus.Json;
using Nautilus.Crafting;
using static CraftData;
using System.Collections.Generic;
using BepInEx.Logging;

namespace CameraDroneStasisUpgrade;

[BepInPlugin("EldritchCarMaker.CameraDroneStasisUpgrade", "Camera Drone Stasis Upgrade", "1.1.0")]
[BepInDependency("EldritchCarMaker.CameraDroneUpgrades")]
public class QMod : BaseUnityPlugin
{
    private static Assembly assembly = Assembly.GetExecutingAssembly();
    public static ManualLogSource Logger { get; private set; }
    internal static TechType moduleTechType;
    public static Config config { get; } = OptionsPanelHandler.RegisterModOptions<Config>();

    public void Awake()
    {
        Logger = base.Logger;
        var CyclopsLockers = ($"EldritchCarMaker_{assembly.GetName().Name}");
        Logger.LogInfo($"Patching {CyclopsLockers}");
        Harmony harmony = new Harmony(CyclopsLockers);
        harmony.PatchAll(assembly);

        var item = new CameraDroneUpgradeModule("MapRoomCameraStasisUpgrade", "Drone Stasis Upgrade", "Allows drones to activate a stasis sphere");
        item.assetsName = "StasisUpgrade";
        item.techData = new RecipeData()
        {
            craftAmount = 1,
            Ingredients = new List<Ingredient>(new Ingredient[]
                    {
                        new Ingredient(TechType.Magnetite, 2),
                        new Ingredient(TechType.ComputerChip, 1),
                        new Ingredient(TechType.WiringKit, 1)
                    }
                )
        };
        item.Patch();
        moduleTechType = item.TechType;

        var stasis = new StasisFunctionality();
        stasis.upgrade = Registrations.RegisterDroneUpgrade("DroneSpeedUpgrade", item.TechType, stasis.SetUp);

        Logger.LogInfo("Patched successfully!");
    }
}
[Menu("Camera Drone Stasis Upgrade")]
public class Config : ConfigFile
{
    [Keybind("Stasis key", Tooltip = "keybind for activating a stasis sphere")]
    public KeyCode stasisKey = KeyCode.F;
}
