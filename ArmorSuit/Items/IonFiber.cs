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
using Nautilus.Assets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Assets.Gadgets;


namespace ArmorSuit.Items
{
    internal static class IonFiber
    {
        public static TechType TType { get; private set; }
        public static void Patch()
        {
            var customPrefab = new CustomPrefab("IonFiber", "Ion Fibers", "Special fibers made using precursor energy sources which have a miraculous ability to adapt", GetItemSprite());
            customPrefab.SetGameObject(new CloneTemplate(customPrefab.Info, TechType.AramidFibers));

            customPrefab.SetRecipe(GetBlueprintRecipe()).WithFabricatorType(CraftTree.Type.Fabricator).StepsToFabricatorTab = new[] { "Resources", "AdvancedMaterials" };
            customPrefab.SetUnlock(TechType.PrecursorIonCrystal).WithPdaGroupCategory(TechGroup.Resources, TechCategory.AdvancedMaterials);

            customPrefab.Register();
            TType = customPrefab.Info.TechType;
        }
        public static Sprite GetItemSprite()
        {
            return SpriteManager.Get(TechType.AramidFibers);
        }
        public static RecipeData GetBlueprintRecipe()
        {
            return new RecipeData()
            {
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient(TechType.AramidFibers, 2),
                    new Ingredient(TechType.PrecursorIonCrystal, 1)
                },
                craftAmount = 1
            };
        }
    }
}
