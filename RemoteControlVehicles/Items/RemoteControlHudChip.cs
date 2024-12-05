using System.Collections.Generic;
#if SN1
using SMLHelper.V2.Assets;
using SMLHelper.V2.Crafting;
using RecipeData = SMLHelper.V2.Crafting.TechData;
using SMLHelper.V2.Utility;
using Logger = QModManager.Utility.Logger;
#else
using Nautilus;
using Nautilus.Assets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Crafting;
using Nautilus.Utility;
using Nautilus.Assets.Gadgets;
#endif
using System.Reflection;
using Sprite = Atlas.Sprite;
using System.IO;
using UnityEngine;
using static CraftData;

namespace RemoteControlVehicles
{
#if !SN1
    internal class RemoteControlHudChip
    {
        public static TechType thisTechType { get; set; }
        public void Patch()
        {
            var folder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");
            var sprite = ImageUtils.LoadSpriteFromFile(Path.Combine(folder, "HudChipRemoteControl.png"));

            var customPrefab = new CustomPrefab("VehicleRemoteControlChip", "Vehicle Remote Control Chip", "Allows use of remote control modules", sprite);

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
            }).WithFabricatorType(CraftTree.Type.Fabricator).WithStepsToFabricatorTab("Personal", "Equipment");
            customPrefab.SetEquipment(EquipmentType.Chip).QuickSlotType = QuickSlotType.Passive;
            customPrefab.SetPdaGroupCategory(TechGroup.Personal, TechCategory.Equipment);
            customPrefab.SetUnlock(TechType.Seamoth);
            customPrefab.SetGameObject(new CloneTemplate(customPrefab.Info, TechType.SeamothSonarModule));

            customPrefab.Register();
            thisTechType = customPrefab.Info.TechType;
        }
    }
#else
    internal class RemoteControlHudChip : Equipable
    {
        public static TechType thisTechType;

        public override string AssetsFolder => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");

        public RemoteControlHudChip() : base("VehicleRemoteControlChip", "Vehicle Remote Control Chip", "Allows use of remote control modules")
        {
            OnFinishedPatching += () =>
            {
                thisTechType = TechType;
            };
        }

        public override EquipmentType EquipmentType => EquipmentType.Chip;
        public override TechType RequiredForUnlock => TechType.Seamoth;
        public override TechGroup GroupForPDA => TechGroup.VehicleUpgrades;
        public override TechCategory CategoryForPDA => TechCategory.VehicleUpgrades;
        public override CraftTree.Type FabricatorType => CraftTree.Type.Fabricator;
        public override string[] StepsToFabricatorTab => new string[] { "Personal", "Equipment"};
        public override float CraftingTime => 3f;
        public override QuickSlotType QuickSlotType => QuickSlotType.Passive;
        protected override Sprite GetItemSprite()
        {
            return ImageUtils.LoadSpriteFromFile(Path.Combine(AssetsFolder, "HudChipRemoteControl.png"));
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
            var prefab = CraftData.GetPrefabForTechType(TechType.MapRoomHUDChip);
            var obj = GameObject.Instantiate(prefab);
            return obj;
        }
    }
#endif
    }