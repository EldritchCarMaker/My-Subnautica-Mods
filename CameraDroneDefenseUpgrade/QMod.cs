using System.Reflection;
using HarmonyLib;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using BepInEx;
using Nautilus.Handlers;
using Nautilus.Json;
using BepInEx.Logging;
using Nautilus.Crafting;
using static CraftData;
using Nautilus.Utility;
using CameraDroneUpgrades.API;
using Nautilus.Options.Attributes;

namespace CameraDroneDefenseUpgrade;
[BepInPlugin("EldritchCarMaker.CameraDroneDefenseUpgrade", "CameraDroneDefenseUpgrade", "1.1.0")]
[BepInDependency("EldritchCarMaker.CameraDroneUpgrades")]
public class QMod : BaseUnityPlugin
{
    private static Assembly assembly = Assembly.GetExecutingAssembly();
    private static string modPath = Path.GetDirectoryName(assembly.Location);
    private static string AssetsFolder = Path.Combine(modPath, "Assets");
    public static TechType itemType;
    public static Config config { get; } = OptionsPanelHandler.RegisterModOptions<Config>();
    public static ManualLogSource Logger;
    public void Awake()
    {
        Logger = base.Logger;
        var CyclopsLockers = ($"EldritchCarMaker_{assembly.GetName().Name}");
        Logger.LogInfo($"Patching {CyclopsLockers}");
        Harmony harmony = new Harmony(CyclopsLockers);
        harmony.PatchAll(assembly);

        var recipe = new RecipeData()
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
        var sprite = ImageUtils.LoadSpriteFromFile(Path.Combine(AssetsFolder, "DefenseUpgrade.png"));

        var item = new CameraDroneUpgradeModule(
            "MapRoomCameraDefenseUpgrade", 
            "Drone Defense Upgrade", 
            "Drones now automatically zap creatures that attempt to grab them", 
            sprite, 
            techData: recipe
            );

        item.Patch();
        itemType = item.TechType;

        Registrations.RegisterDroneUpgrade("DroneDefenseUpgrade", item.TechType, null).key = KeyCode.None;

        Logger.LogInfo("Patched successfully!");
    }
}
[Menu("Camera Drone Defense Upgrade")]
public class Config : ConfigFile
{
    [Slider("Defense Upgrade Zap Damage", Format = "{0:F2}", Min = 0.0F, Max = 60.0f, DefaultValue = 30.0F, Step = 0.5F, Tooltip = "How much damage the defense upgrade does when zapping something")]
    public float damageAmount = 30f;
}
