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

namespace CameraDroneShieldUpgrade;

[BepInPlugin("EldritchCarMaker.CameraDroneShieldUpgrade", "Camera Drone Shield Upgrade", "1.1.0")]
[BepInDependency("EldritchCarMaker.CameraDroneUpgrades")]
public class QMod : BaseUnityPlugin
{
    private static Assembly assembly = Assembly.GetExecutingAssembly();
    internal static TechType moduleTechType;
    public static Config config { get; } = OptionsPanelHandler.RegisterModOptions<Config>();

    public static ShieldFunctionality shield;

    public void Awake()
    {
        var CyclopsLockers = ($"EldritchCarMaker_{assembly.GetName().Name}");
        Logger.LogInfo($"Patching {CyclopsLockers}");
        Harmony harmony = new Harmony(CyclopsLockers);
        harmony.PatchAll(assembly);

        var item = new CameraDroneUpgradeModule("MapRoomCameraShieldUpgrade", "Drone Shield Upgrade", "Allows drones to use a small energy shield");
        item.assetsName = "ShieldUpgrade";
        item.techData = new RecipeData()
        {
            craftAmount = 1,
            Ingredients = new List<Ingredient>(new Ingredient[]
                    {
                        new Ingredient(TechType.Polyaniline, 1),
                        new Ingredient(TechType.ComputerChip, 1),
                        new Ingredient(TechType.WiringKit, 1)
                    }
                )
        };
        item.Patch();
        moduleTechType = item.TechType;

        shield = new ShieldFunctionality();
        shield.upgrade = Registrations.RegisterDroneUpgrade("DroneShieldUpgrade", item.TechType, shield.SetUp);

        Logger.LogInfo("Patched successfully!");
    }
}
[Menu("Camera Drone Shield Upgrade")]
public class Config : ConfigFile
{
    [Keybind("Shield", Tooltip = "keybind for toggling a shield for camera drones")]
    public KeyCode shieldKey = KeyCode.X;
}
