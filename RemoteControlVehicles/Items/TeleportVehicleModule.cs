using System.Collections.Generic;
using System.Reflection;
using Sprite = Atlas.Sprite;
using System.IO;
using UnityEngine;
#if SN1
using RecipeData = SMLHelper.V2.Crafting.TechData;
using SMLHelper.V2.Utility;
using Logger = QModManager.Utility.Logger;
using SMLHelper.V2.Assets;
using SMLHelper.V2.Crafting;
#else
using Nautilus.Assets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Crafting;
using Nautilus.Utility;
using Nautilus.Assets.Gadgets;
#endif
using MoreCyclopsUpgrades.API.Upgrades;
using MoreCyclopsUpgrades.API;
using static CraftData;

namespace RemoteControlVehicles
{
    public class CyclopsRemoteControlHandler : UpgradeHandler
    {
        internal static readonly List<CyclopsRemoteControlHandler> AllHandlers = new List<CyclopsRemoteControlHandler>();
        internal static SubRoot TrackedSub;

        public CyclopsRemoteControlHandler(TechType techType, SubRoot cyclops) : base(techType, cyclops)
        {
            AllHandlers.Add(this); // on construction, add this handler to the static list (this only happens once)

            IsAllowedToAdd = (TechType, verbose) => {
                if (AnySubHasRemoteUpgrade())
                {
                    ErrorMessage.AddMessage("Only one vehicle can have this module equipped at a time!");
                    return false;
                }
                return true;
            };

            OnFinishedUpgrades = () =>
            {
                if(this.HasUpgrade)
                    TrackedSub = cyclops;
                else if (!AnySubHasRemoteUpgrade())
                    TrackedSub = null;
            };
        }

        private static bool AnySubHasRemoteUpgrade()
        {
            foreach(var handler in AllHandlers)
            {
                if(handler.HasUpgrade)
                    return true;
            }
            return false;
        }
    }
#if !SN1
    public class TeleportVehicleModule
    {
        public static TechType thisTechType;
        public void Patch()
        {
            var folder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");
            var sprite = ImageUtils.LoadSpriteFromFile(Path.Combine(folder, "Seamoth_remote_module.png"));

            var customPrefab = new CustomPrefab("VehicleRemoteControl", "Vehicle Remote Control", "Allows remote control over vehicle", sprite);

            customPrefab.SetRecipe(new RecipeData()
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>(new Ingredient[]
                    {
                        new Ingredient(TechType.Magnetite, 2),
                        new Ingredient(TechType.ComputerChip, 2),
                        new Ingredient(TechType.WiringKit, 1)

                    }
                )
            }).WithFabricatorType(CraftTree.Type.SeamothUpgrades).WithStepsToFabricatorTab("CommonModules");
            customPrefab.SetEquipment(EquipmentType.VehicleModule).QuickSlotType = QuickSlotType.Passive;
            customPrefab.SetPdaGroupCategory(TechGroup.VehicleUpgrades, TechCategory.VehicleUpgrades);
            customPrefab.SetUnlock(TechType.Seamoth);
            customPrefab.SetGameObject(new CloneTemplate(customPrefab.Info, TechType.SeamothSonarModule));

            customPrefab.Register();
            thisTechType = customPrefab.Info.TechType;
        }
    }
#else
    public class TeleportVehicleModule :  Equipable
    {
        public static TechType thisTechType;

        public override string AssetsFolder => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");

        public TeleportVehicleModule() : base("VehicleRemoteControl", "Vehicle Remote Control", "Allows remote control over vehicle")
        {
            OnFinishedPatching += () =>
            {
                thisTechType = TechType;
            };
        }

        public override EquipmentType EquipmentType => EquipmentType.VehicleModule;
        public override TechType RequiredForUnlock => TechType.Seamoth;
        public override TechGroup GroupForPDA => TechGroup.VehicleUpgrades;
        public override TechCategory CategoryForPDA => TechCategory.VehicleUpgrades;
        public override CraftTree.Type FabricatorType => CraftTree.Type.SeamothUpgrades;
        public override string[] StepsToFabricatorTab => new string[] { "CommonModules" };
        public override float CraftingTime => 3f;
        public override QuickSlotType QuickSlotType => QuickSlotType.Passive;
        protected override Sprite GetItemSprite()
        {
            return ImageUtils.LoadSpriteFromFile(Path.Combine(AssetsFolder, "Seamoth_remote_module.png"));
        }

        protected override RecipeData GetBlueprintRecipe()
        {
            return new RecipeData()
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>(new Ingredient[]
                    {
                        new Ingredient(TechType.Magnetite, 2),
                        new Ingredient(TechType.ComputerChip, 2),
                        new Ingredient(TechType.WiringKit, 1)

                    }
                )
            };
        }

        public override GameObject GetGameObject()
        {
            var prefab = CraftData.GetPrefabForTechType(TechType.SeamothSonarModule);
            var obj = GameObject.Instantiate(prefab);
            return obj;
        }
    }
#endif
    public class CyclopsRemoteControlModule : CyclopsUpgrade
    {
        public static TechType thisTechType;

        public override string AssetsFolder => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");

        public CyclopsRemoteControlModule() : base("CyclopsRemoteControl", "Cyclops Remote Control", "Allows remote control over the cyclops")
        {
            OnFinishedPatching += () =>
            {
                thisTechType = TechType;
            };
        }

        public override TechType RequiredForUnlock => TechType.Cyclops;
        public override CraftTree.Type FabricatorType => CraftTree.Type.CyclopsFabricator;
        public override string[] StepsToFabricatorTab => MCUServices.CrossMod.StepsToCyclopsModulesTabInCyclopsFabricator;
        public override float CraftingTime => 3f;
#if SN1
        protected override Sprite GetItemSprite()
        {
            return ImageUtils.LoadSpriteFromFile(Path.Combine(AssetsFolder, "cyclops_remote_module.png"));
        }
        public override GameObject GetGameObject()
        {
            var prefab = CraftData.GetPrefabForTechType(TechType.CyclopsShieldModule);
            var obj = GameObject.Instantiate(prefab);
            return obj;
        }
#else
        public Sprite GetItemSprite()
        {
            return ImageUtils.LoadSpriteFromFile(Path.Combine(AssetsFolder, "cyclops_remote_module.png"));
        }
#endif

        protected override RecipeData GetBlueprintRecipe()
        {
            return new RecipeData()
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>(new Ingredient[]
                    {
                        new Ingredient(TechType.Magnetite, 2),
                        new Ingredient(TechType.ComputerChip, 2),
                        new Ingredient(TechType.WiringKit, 1)
                    }
                )
            };
        }
    }
}