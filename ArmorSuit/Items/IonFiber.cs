using SMLHelper.V2.Assets;
using SMLHelper.V2.Crafting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ArmorSuit.Items
{
    internal class IonFiber : Craftable
    {
        public static TechType TType { get; private set; }
        public IonFiber() : base("IonFiber", "Ion Fibers", "Special fibers made using precursor energy sources which have a miraculous ability to adapt")
        {
            OnFinishedPatching += () => TType = TechType;
        }
        protected override Atlas.Sprite GetItemSprite()
        {
            return SpriteManager.Get(TechType.AramidFibers);
        }
        protected override TechData GetBlueprintRecipe()
        {
            return new TechData()
            {
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient(TechType.AramidFibers, 2),
                    new Ingredient(TechType.PrecursorIonCrystal, 1)
                },
                craftAmount = 1
            };
        }
        public override GameObject GetGameObject()
        {
            return CraftData.InstantiateFromPrefab(TechType.AramidFibers);
        }
        public override TechCategory CategoryForPDA => TechCategory.AdvancedMaterials;
        public override CraftTree.Type FabricatorType => CraftTree.Type.Fabricator;
        public override TechType RequiredForUnlock => TechType.PrecursorIonCrystal;
        public override string[] StepsToFabricatorTab => new[] { "Resources", "AdvancedMaterials" };
    }
}
