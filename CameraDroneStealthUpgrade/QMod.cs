using System.Reflection;
using HarmonyLib;
using UnityEngine;
using System.IO;
using CameraDroneUpgrades.API;
using BepInEx;
using Nautilus.Handlers;
using Nautilus.Crafting;
using static CraftData;
using System.Collections.Generic;
using Nautilus.Options.Attributes;
using Nautilus.Json;

namespace CameraDroneStealthUpgrade;

[BepInPlugin("EldritchCarMaker.CameraDroneStealthUpgrade", "Camera Drone Stealth Upgrade", "1.1.0")]
[BepInDependency("EldritchCarMaker.CameraDroneUpgrades")]
public class QMod : BaseUnityPlugin
{
    private static Assembly assembly = Assembly.GetExecutingAssembly();
    private static string modPath = Path.GetDirectoryName(assembly.Location);
    public static Config config { get; } = OptionsPanelHandler.RegisterModOptions<Config>();

    public void Awake()
    {
        var CyclopsLockers = ($"EldritchCarMaker_{assembly.GetName().Name}");
        Logger.LogInfo($"Patching {CyclopsLockers}");
        Harmony harmony = new Harmony(CyclopsLockers);
        harmony.PatchAll(assembly);

        var item = new CameraDroneUpgradeModule("MapRoomCameraStealthUpgrade", "Drone Stealth Upgrade", "Drones are now cloaked from predators vision");
        item.assetsName = "StealthUpgrade";
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

        var Stealth = new StealthFunctionality();
        Stealth.upgrade = Registrations.RegisterDroneUpgrade("DroneStealthUpgrade", item.TechType, Stealth.SetUp);

        Logger.LogInfo("Patched successfully!");
    }
}
[Menu("Camera Drone Stealth Upgrade")]
public class Config : ConfigFile
{
    [Keybind("Stealth mode", Tooltip = "keybind for toggling stealth for camera drones")]
    public KeyCode stealthKey = KeyCode.T;
}
