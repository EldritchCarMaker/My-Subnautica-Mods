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


namespace ArmorSuit.Items
{
    internal class IonFiber :
#if SN1
        Craftable
#else
        Equipable
#endif
    {
        public static TechType TType { get; private set; }
        public IonFiber() : base("IonFiber", "Ion Fibers", "Special fibers made using precursor energy sources which have a miraculous ability to adapt")
        {
            OnFinishedPatching += () => TType = TechType;
        }
        protected override Sprite GetItemSprite()
        {
            return SpriteManager.Get(TechType.AramidFibers);
        }
        protected override RecipeData GetBlueprintRecipe()
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
#if SN1
        public override GameObject GetGameObject()
        {
            return CraftData.InstantiateFromPrefab(TechType.AramidFibers);
        }
#endif
        public override IEnumerator GetGameObjectAsync(IOut<GameObject> gameObject)
        {
            var task = CraftData.GetPrefabForTechTypeAsync(TechType.AramidFibers);
            yield return task;
            gameObject.Set(task.GetResult());
        }
        public override TechCategory CategoryForPDA => TechCategory.AdvancedMaterials;
        public override CraftTree.Type FabricatorType => CraftTree.Type.Fabricator;
        public override TechType RequiredForUnlock => TechType.PrecursorIonCrystal;
        public override string[] StepsToFabricatorTab => new[] { "Resources", "AdvancedMaterials" };
    }
}
