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

namespace CameraDroneSpeedUpgrade;

[BepInPlugin("EldritchCarMaker.CameraDroneSpeedUpgrade", "Camera Drone Speed Upgrade", "1.1.0")]
[BepInDependency("EldritchCarMaker.CameraDroneUpgrades")]
public class QMod : BaseUnityPlugin
{
    private static Assembly assembly = Assembly.GetExecutingAssembly();
    internal static TechType moduleTechType;
    public static Config config { get; } = OptionsPanelHandler.RegisterModOptions<Config>();

    public void Awake()
    {
        var CyclopsLockers = ($"EldritchCarMaker_{assembly.GetName().Name}");
        Logger.LogInfo($"Patching {CyclopsLockers}");
        Harmony harmony = new Harmony(CyclopsLockers);
        harmony.PatchAll(assembly);

        var item = new CameraDroneUpgradeModule("MapRoomCameraSpeedUpgrade", "Drone Speed Upgrade", "Allows drones to use a small speed boost to move faster");
        item.assetsName = "SpeedUpgrade";
        item.techData = new RecipeData()
        {
            craftAmount = 1,
            Ingredients = new List<Ingredient>(new Ingredient[]
                    {
                        new Ingredient(TechType.Lubricant, 1),
                        new Ingredient(TechType.WiringKit, 1)
                    }
                )
        };
        item.Patch();
        moduleTechType = item.TechType;

        var Speed = new SpeedFunctionality();
        Speed.upgrade = Registrations.RegisterDroneUpgrade("DroneSpeedUpgrade", item.TechType, Speed.SetUp);

        Logger.LogInfo("Patched successfully!");
    }
}
[Menu("Camera Drone Speed Upgrade")]
public class Config : ConfigFile
{
    [Keybind("Speed key", Tooltip = "keybind for using speed boost for drones")]
    public KeyCode speedKey = KeyCode.F;
}
