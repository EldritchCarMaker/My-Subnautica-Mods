using System.Reflection;
using HarmonyLib;
using QModManager.API.ModLoading;
using Logger = QModManager.Utility.Logger;
using UnityEngine;
using SMLHelper.V2.Json;
using SMLHelper.V2.Options.Attributes;
using SMLHelper.V2.Handlers;
using System.IO;
using System.Collections.Generic;
using SMLHelper.V2.Assets;
using CameraDroneUpgrades.API;
using SMLHelper.V2.Crafting;
using SMLHelper.V2.Utility;

namespace CameraDroneDefenseUpgrade
{
    [QModCore]
    public static class QMod
    {
        private static Assembly assembly = Assembly.GetExecutingAssembly();
        private static string modPath = Path.GetDirectoryName(assembly.Location);
        private static string AssetsFolder = Path.Combine(modPath, "Assets");
        public static TechType itemType;
        public static Config config { get; } = OptionsPanelHandler.RegisterModOptions<Config>();
        [QModPatch]
        public static void Patch()
        {
            var CyclopsLockers = ($"EldritchCarMaker_{assembly.GetName().Name}");
            Logger.Log(Logger.Level.Info, $"Patching {CyclopsLockers}");
            Harmony harmony = new Harmony(CyclopsLockers);
            harmony.PatchAll(assembly);

            var recipe = new TechData()
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
                recipe
                );

            item.Patch();
            itemType = item.TechType;

            Registrations.RegisterDroneUpgrade("DroneDefenseUpgrade", item.TechType, null).key = KeyCode.None;

            Logger.Log(Logger.Level.Info, "Patched successfully!");
        }
    }
    [Menu("Camera Drone Defense Upgrade")]
    public class Config : ConfigFile
    {
        [Slider("Defense Upgrade Zap Damage", Format = "{0:F2}", Min = 0.0F, Max = 60.0f, DefaultValue = 30.0F, Step = 0.5F, Tooltip = "How much damage the defense upgrade does when zapping something")]
        public float damageAmount = 30f;
    }
}
