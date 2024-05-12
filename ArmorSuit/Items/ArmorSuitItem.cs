using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
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
using UnityEngine;
using ArmorSuit.Items;
using System.Collections;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;

namespace ArmorSuit
{
    internal static class ArmorSuitItem
    {
        public static TechType thisTechType;

        public static void Patch()
        {
            var customPrefab = new CustomPrefab("ArmorSuitItem", "Armor Suit", "A high tech adaptive suit which gives high damage reduction to a specific damage type", GetItemSprite());
            customPrefab.SetGameObject(new CloneTemplate(customPrefab.Info, TechType.ReinforcedDiveSuit));
            customPrefab.Info.WithSizeInInventory(new(2, 2));

            customPrefab.SetRecipe(GetBlueprintRecipe()).WithFabricatorType(CraftTree.Type.Fabricator).StepsToFabricatorTab = new[] { "Personal", "Equipment" };
            customPrefab.SetUnlock(TechType.ReinforcedDiveSuit).WithPdaGroupCategory(TechGroup.Personal, TechCategory.Equipment);
            customPrefab.SetEquipment(EquipmentType.Body);
            

            customPrefab.Register();
            thisTechType = customPrefab.Info.TechType;
        }


        public static Sprite GetItemSprite()
        {
            return ImageUtils.LoadSpriteFromFile(Path.Combine(ArmorSuitMono.AssetsFolder, "ArmorSuit.png"));
        }

        public static RecipeData GetBlueprintRecipe()
        {
            return new RecipeData()
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>(new Ingredient[]
                    {
                        new Ingredient(IonFiber.TType, 2),
                        new Ingredient(TechType.ReinforcedGloves, 1),
                        new Ingredient(TechType.ReinforcedDiveSuit, 1)
                    }
                ),
                LinkedItems = new List<TechType>() { ArmorGlovesItem.techType }
            };
        }
    }
}
