using System.Reflection;
using HarmonyLib;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using CameraDroneUpgrades.API;
using BepInEx;
using Nautilus.Handlers;
using Nautilus.Json;
using Nautilus.Options.Attributes;
using Nautilus.Crafting;
using static CraftData;

namespace CameraDroneRepairUpgrade;

[BepInPlugin("EldritchCarMaker.CameraDroneRepairUpgrade", "Camera Drone Repair Upgrade", "1.1.0")]
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

        var item = new CameraDroneUpgradeModule("MapRoomCameraRepairUpgrade", "Drone Repair Upgrade", "Allows drones to repair objects they're looking at");
        item.assetsName = "RepairUpgrade";
        item.techData = new RecipeData()
        {
            craftAmount = 1,
            Ingredients = new List<Ingredient>(new Ingredient[]
                    {
                        new Ingredient(TechType.Welder, 1),
                        new Ingredient(TechType.ComputerChip, 1),
                        new Ingredient(TechType.WiringKit, 1)
                    }
                )
        };
        item.Patch();
        moduleTechType = item.TechType;

        var shield = new RepairFunctionality();
        shield.upgrade = Registrations.RegisterDroneUpgrade("DroneRepairUpgrade", item.TechType, shield.SetUp);

        Logger.LogInfo("Patched successfully!");
    }
}
[Menu("Camera Drone Repair Upgrade")]
public class Config : ConfigFile
{
    [Keybind("Repair", Tooltip = "keybind for repairing objects while using camera drones")]
    public KeyCode repairKey = KeyCode.R;
}
