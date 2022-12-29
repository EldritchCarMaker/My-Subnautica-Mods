using EquivalentExchange.Monobehaviours;
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
using RecipeData = SMLHelper.V2.Crafting.TechData;
using Sprite = Atlas.Sprite;
#endif

namespace EquivalentExchange.Constructables
{
    internal class AutomaticItemConverterConstructable : Buildable
    {
        public AutomaticItemConverterConstructable() : base("AutomaticItemConverterConstructable", "Automatic Item Converter", "A small container that automatically, and constantly, converts ECM into a single, specified, item")
        {
        }
        public override TechGroup GroupForPDA { get; } = TechGroup.InteriorModules;
        public override TechCategory CategoryForPDA { get; } = TechCategory.InteriorModule;
        public override TechType RequiredForUnlock { get; } = TechType.PrecursorIonCrystal;

        protected override RecipeData GetBlueprintRecipe()
        {
            return new RecipeData
            {
                Ingredients =
                {
                    new Ingredient(TechType.PrecursorIonCrystal, 1),
                    new Ingredient(TechType.Titanium, 1),
                    new Ingredient(TechType.Lubricant, 1)
                }
            };
        }
#if SN1
        public override GameObject GetGameObject()
        {
            var obj = CraftData.InstantiateFromPrefab(TechType.MedicalCabinet);
            obj.AddComponent<AutomaticItemConverter>();
            return obj;
        }
#endif
        public override IEnumerator GetGameObjectAsync(IOut<GameObject> gameObject)
        {
            var task =  CraftData.GetPrefabForTechTypeAsync(TechType.MedicalCabinet);
            yield return task;
            gameObject.Set(task.GetResult().EnsureComponent<AutomaticItemConverter>().gameObject);
        }
    }
}