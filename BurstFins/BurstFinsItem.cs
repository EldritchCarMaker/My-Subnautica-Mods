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
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;

namespace BurstFins
{
    internal class BurstFinsItem
    {
        public static TechType thisTechType;
        public static Sprite sprite = SpriteManager.Get(TechType.UltraGlideFins);
        public static void Patch()
        {
            var customPrefab = new CustomPrefab("BurstFinsItem", "Burst Fins", "Allows a short burst of speed before going on cooldown", GetItemSprite());
            customPrefab.SetGameObject(new CloneTemplate(customPrefab.Info, TechType.UltraGlideFins));

            customPrefab.SetRecipe(GetBlueprintRecipe()).WithFabricatorType(CraftTree.Type.Fabricator).StepsToFabricatorTab = new[] { "Personal", "Equipment" };
            customPrefab.SetUnlock(TechType.UltraGlideFins).WithPdaGroupCategory(TechGroup.Personal, TechCategory.Equipment);
            customPrefab.SetEquipment(EquipmentType.Foots);

            customPrefab.Register();
            thisTechType = customPrefab.Info.TechType;
        }

        public static Sprite GetItemSprite()
        {
            var ChangedSprite = sprite;
#if SN
            ChangedSprite.size = new Vector2(-ChangedSprite.size.x, ChangedSprite.size.y);
#endif
            return ChangedSprite;
        }

        public static RecipeData GetBlueprintRecipe()
        {
            return new RecipeData()
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>(new Ingredient[]
                    {
                        new Ingredient(TechType.UraniniteCrystal, 3),
#if SN
                        new Ingredient(TechType.ToyCar, 1),
#else
                        new Ingredient(TechType.HoverbikeJumpModule, 1),
#endif
                        new Ingredient(TechType.UltraGlideFins, 1)
                    }
                )
            };
        }
    }
}
