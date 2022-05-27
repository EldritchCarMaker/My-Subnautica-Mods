using SMLHelper.V2.Assets;
using SMLHelper.V2.Crafting;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Sprite = Atlas.Sprite;
using RecipeData = SMLHelper.V2.Crafting.TechData;
using SMLHelper.V2.Utility;
using UnityEngine;

namespace RechargerChips.Items
{
    internal class ComboChargerChip : Equipable
    {
        public static TechType thisTechType;

        public override string AssetsFolder => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");

        public ComboChargerChip() : base("ComboChargerChip", "Combo Charger Chip", "Combines the solar and thermal charger chips")
        {
            OnFinishedPatching += () =>
            {
                thisTechType = TechType;
            };
        }

        public override EquipmentType EquipmentType => EquipmentType.Chip;
        public override TechType RequiredForUnlock => ThermalChargerChip.thisTechType;
        public override TechGroup GroupForPDA => TechGroup.Personal;
        public override TechCategory CategoryForPDA => TechCategory.Equipment;
        public override CraftTree.Type FabricatorType => CraftTree.Type.Workbench;
        public override string[] StepsToFabricatorTab => new string[] {};
        public override float CraftingTime => 3f;
        public override QuickSlotType QuickSlotType => QuickSlotType.Passive;
        protected override Sprite GetItemSprite()
        {
            return ImageUtils.LoadSpriteFromFile(Path.Combine(AssetsFolder, "chip_solar_thermal.png"));
        }

        protected override RecipeData GetBlueprintRecipe()
        {
            return new RecipeData()
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>(new Ingredient[]
                    {
                        new Ingredient(SolarChargerChip.thisTechType, 1),
                        new Ingredient(ThermalChargerChip.thisTechType, 1),
                        new Ingredient(TechType.AdvancedWiringKit, 1)
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
}
