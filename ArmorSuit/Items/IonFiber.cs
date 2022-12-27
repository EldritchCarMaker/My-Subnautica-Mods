using SMLHelper.V2.Assets;
using SMLHelper.V2.Crafting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
#if SN
using Sprite = Atlas.Sprite;
using RecipeData = SMLHelper.V2.Crafting.TechData;
#endif

namespace ArmorSuit.Items
{
    internal class IonFiber : Craftable
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
