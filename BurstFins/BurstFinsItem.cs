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
using EquippableItemIcons.API.SecretSMLNautilusAPIDontTouch;
using static CraftData;
#endif
#endif
using System.Collections;
using UnityEngine;
using System.Collections.Generic;

namespace BurstFins
{
    internal class BurstFinsItem : Equipable
    {
        public static TechType thisTechType;
        public static Sprite sprite = SpriteManager.Get(TechType.UltraGlideFins);

        public BurstFinsItem() : base("BurstFinsItem", "Burst Fins", "Allows a short burst of speed before going on cooldown")
        {
            OnFinishedPatching += () =>
            {
                thisTechType = TechType;
            };
        }

        public override EquipmentType EquipmentType => EquipmentType.Foots;
        public override TechType RequiredForUnlock => TechType.UltraGlideFins;
        public override TechGroup GroupForPDA => TechGroup.Personal;
        public override TechCategory CategoryForPDA => TechCategory.Equipment;
        public override CraftTree.Type FabricatorType => CraftTree.Type.Fabricator;
        public override string[] StepsToFabricatorTab => new string[] { "Personal", "Equipment" };
        public override float CraftingTime => 3f;
        public override QuickSlotType QuickSlotType => QuickSlotType.Passive;
        protected override Sprite GetItemSprite()
        {
            var ChangedSprite = sprite;
#if SN
            ChangedSprite.size = new Vector2(-ChangedSprite.size.x, ChangedSprite.size.y);
#endif
            return ChangedSprite;
        }

        protected override RecipeData GetBlueprintRecipe()
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
#if SN1
        public override GameObject GetGameObject()
        {
            var prefab = CraftData.GetPrefabForTechType(TechType.UltraGlideFins);
            var obj = GameObject.Instantiate(prefab);
            return obj;
        }
#endif
        public override IEnumerator GetGameObjectAsync(IOut<GameObject> gameObject)
        {
            var task = CraftData.GetPrefabForTechTypeAsync(TechType.UltraGlideFins);
            yield return task;
            gameObject.Set(task.GetResult());
        }
    }
}
