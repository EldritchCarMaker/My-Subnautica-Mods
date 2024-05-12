#if SN
using Sprite = Atlas.Sprite;
#if SN1
using RecipeData = SMLHelper.V2.Crafting.TechData;
using SMLHelper.V2.Assets;
using SMLHelper.V2.Crafting;
using SMLHelper.V2.Utility;
#else
using Nautilus.Crafting;
using Nautilus.Utility;
using static CraftData;
#endif
#endif
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;

namespace SpyWatch
{
    internal class SpyWatchItem
    {
        public static TechType thisTechType;
        public static Sprite sprite = ImageUtils.LoadSpriteFromFile(Path.Combine(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets"), "SpyWatchItem.png"));
        public string AssetsFolder => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");
        public static void Patch()
        {
            var customPrefab = new CustomPrefab("SpyWatchItem", "Spy Watch", "Allows short length invisibility using membrain dna", sprite);
            customPrefab.SetGameObject(new CloneTemplate(customPrefab.Info, TechType.UltraGlideFins));

            customPrefab.SetRecipe(GetBlueprintRecipe()).WithFabricatorType(CraftTree.Type.Fabricator).StepsToFabricatorTab = new[] { "Personal", "Equipment" };
            customPrefab.SetUnlock(TechType.AdvancedWiringKit).WithPdaGroupCategory(TechGroup.Personal, TechCategory.Equipment);
            customPrefab.SetEquipment(EquipmentType.Chip);

            customPrefab.Register();
            thisTechType = customPrefab.Info.TechType;
        }

        public static RecipeData GetBlueprintRecipe()
        {
            return new RecipeData()
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>(new Ingredient[]
                    {
                        new Ingredient(TechType.Glass, 1),
#if SN
                        new Ingredient(TechType.MembrainTreeSeed, 1),
#else

#endif
                        new Ingredient(TechType.AdvancedWiringKit, 1),
                        new Ingredient(TechType.Battery, 1)
                    }
                )
            };
        }
    }
}
