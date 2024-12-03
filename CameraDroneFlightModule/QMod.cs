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

namespace CameraDroneFlightUpgrade;

[BepInPlugin("EldritchCarMaker.CameraDroneFlightUpgrade", "Camera Drone Flight Upgrade", "1.1.0")]
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

        var item = new CameraDroneUpgradeModule("MapRoomCameraFlightUpgrade", "Drone Flight Upgrade", "Allows drones to fly/hover");
        item.assetsName = "FlightUpgrade";
        item.techData = new RecipeData()
        {
            craftAmount = 1,
            Ingredients = new List<Ingredient>(new Ingredient[]
                    {
                        new Ingredient(TechType.ComputerChip, 1),
                        new Ingredient(TechType.WiringKit, 1)
                    }
                )
        };
        item.Patch();
        moduleTechType = item.TechType;

        var shield = new FlightFunctionality();
        shield.upgrade = Registrations.RegisterDroneUpgrade("DroneFlightUpgrade", item.TechType, shield.SetUp);

        Logger.LogInfo("Patched successfully!");
    }
}
[Menu("Camera Drone Flight Upgrade")]
public class Config : ConfigFile
{
    [Keybind("Flight", Tooltip = "keybind for toggling hover mode while using camera drones")]
    public KeyCode flightKey = KeyCode.H;

    [Slider("Energy Drain", Format = "{0:F2}", Min = 0, Max = 1, DefaultValue = 0.5F, Step = 0.1F, Tooltip = "Energy drain of hovering")]
    public float powerDrain = 0.5f;
}
