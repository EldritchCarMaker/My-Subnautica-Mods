using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Crafting;
using Nautilus.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static CraftData;

#if SN
using Sprite = Atlas.Sprite;
#endif

namespace EquivalentExchange
{
    internal class AutoItemConversionChip
    {
        public static void Patch()
        {
            var customPrefab = new CustomPrefab("AutoItemConversionChip", "Auto Item Conversion Chip", "A chip to allow for automatic item conversion as required for fabrication with unlimited range-- ONLY WORKS WHEN EASY CRAFT IS INSTALLED", GetItemSprite());

            customPrefab.SetGameObject(new CloneTemplate(customPrefab.Info, TechType.MapRoomHUDChip));

            customPrefab.SetRecipe(GetBlueprintRecipe()).WithStepsToFabricatorTab(StepsToFabricatorTab).FabricatorType = CraftTree.Type.Fabricator;
            customPrefab.SetPdaGroupCategory(TechGroup.Personal, TechCategory.Equipment);
            customPrefab.SetUnlock(TechType.Kyanite);

            customPrefab.SetEquipment(EquipmentType.Chip);

            customPrefab.Register();
            techType = customPrefab.Info.TechType;
        }
        public static TechType techType;


        protected static RecipeData GetBlueprintRecipe()
        {
            return new RecipeData()
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient(TechType.PrecursorIonCrystal, 2),
                    new Ingredient(TechType.ComputerChip, 2),
                    new Ingredient(TechType.Kyanite, 2),
                    new Ingredient(TechType.AdvancedWiringKit, 2),
#if SN
                    new Ingredient(TechType.HatchingEnzymes, 1),
#endif
                }
            };
        }
        public static string AssetsFolder => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");
        public static string[] StepsToFabricatorTab => new string[] { "Personal", "Equipment" };
        protected static Sprite GetItemSprite()
        {
            return ImageUtils.LoadSpriteFromFile(Path.Combine(AssetsFolder, "chip_ecm.png"));
        }
    }
}
