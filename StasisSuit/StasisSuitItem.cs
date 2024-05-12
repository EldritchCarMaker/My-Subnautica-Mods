﻿#if SN
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
using static Atlas;
using Nautilus.Assets.Gadgets;

namespace StasisSuit
{
    internal class StasisSuitItem
    {
        public static TechType thisTechType;
        public static string AssetsFolder => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");
        public static void Patch()
        {
            var customPrefab = new CustomPrefab("StasisSuitItem", "Stasis Suit", "suit that allows you to temporarily pause all internal bodily functions, including thirst and hunger.", GetItemSprite());
            customPrefab.SetGameObject(new CloneTemplate(customPrefab.Info, TechType.UltraGlideFins));
            customPrefab.Info.WithSizeInInventory(new (2, 2));

            customPrefab.SetRecipe(GetBlueprintRecipe()).WithFabricatorType(CraftTree.Type.Fabricator).StepsToFabricatorTab = new[] { "Personal", "Equipment" };
            var unlockType =
#if SN 
                TechType.StasisRifle;
#else
                TechType.Megnetite;
#endif
            customPrefab.SetUnlock(unlockType).WithPdaGroupCategory(TechGroup.Personal, TechCategory.Equipment);
            customPrefab.SetEquipment(EquipmentType.Body);

            customPrefab.Register();
            thisTechType = customPrefab.Info.TechType;
        }
        public static Sprite GetItemSprite()
        {
            return ImageUtils.LoadSpriteFromFile(Path.Combine(AssetsFolder, "StasisSuitItem.png"));
        }

        public static RecipeData GetBlueprintRecipe()
        {
            return new RecipeData()
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>(new Ingredient[]
                    {
                        new Ingredient(TechType.AdvancedWiringKit, 1),
                        new Ingredient(TechType.Magnetite, 2),
                        new Ingredient(TechType.FiberMesh, 2)
                    }
                )
            };
        }
    }
}
