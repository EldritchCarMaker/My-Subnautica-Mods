using SMLHelper.V2.Assets;
using SMLHelper.V2.Crafting;
using SMLHelper.V2.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace EquivalentExchange
{
    internal class AutoItemConversionChip : Equipable
    {
        public static TechType techType;
        public AutoItemConversionChip() : base("AutoItemConversionChip", "Auto Item Conversion Chip", "A chip to allow for automatic item conversion as required for fabrication with unlimited range-- ONLY WORKS WHEN EASY CRAFT IS INSTALLED")
        {
            OnFinishedPatching += () => techType = TechType;
        }

        public override EquipmentType EquipmentType => EquipmentType.Chip;

        protected override TechData GetBlueprintRecipe()
        {
            return new TechData()
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient(TechType.PrecursorIonCrystal, 2),
                    new Ingredient(TechType.ComputerChip, 2),
                    new Ingredient(TechType.Kyanite, 2),
                    new Ingredient(TechType.AdvancedWiringKit, 2),
                    new Ingredient(TechType.HatchingEnzymes, 1),
                }
            };
        }
        public override string AssetsFolder => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");
        public override TechCategory CategoryForPDA => TechCategory.Equipment;
        public override CraftTree.Type FabricatorType => CraftTree.Type.Fabricator;
        public override string[] StepsToFabricatorTab => new string[] { "Personal", "Equipment" };
        public override TechType RequiredForUnlock => TechType.Kyanite;
        public override float CraftingTime => 3f;
        protected override Atlas.Sprite GetItemSprite()
        {
            return ImageUtils.LoadSpriteFromFile(Path.Combine(AssetsFolder, "chip_ecm.png"));
        }
        public override GameObject GetGameObject()
        {
            return CraftData.GetPrefabForTechType(TechType.MapRoomHUDChip);
        }
    }
}
