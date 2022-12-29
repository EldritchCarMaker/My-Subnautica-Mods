using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using EquivalentExchange.Monobehaviours;
using SMLHelper.V2.Assets;
using SMLHelper.V2.Crafting;
using UnityEngine;
#if SN
using RecipeData = SMLHelper.V2.Crafting.TechData;
using Sprite = Atlas.Sprite;
#endif

namespace EquivalentExchange.Constructables
{
    internal class ItemResearchStationConstructable : Buildable
    {
        public ItemResearchStationConstructable() : base("ItemResearchStationConstructable", "Item Research Station", "A station used to research items for the purposes of material exchange")
        {
        }
        public GameObject prefab;
        public override TechGroup GroupForPDA { get; } = TechGroup.InteriorPieces;
        public override TechCategory CategoryForPDA { get; } = TechCategory.InteriorPiece;
        public override TechType RequiredForUnlock { get; } = TechType.PrecursorIonCrystal;

        protected override RecipeData GetBlueprintRecipe()
        {
            return new RecipeData
            {
                Ingredients =
                {
                    new Ingredient(TechType.PrecursorIonCrystal, 1),
#if SN
                    new Ingredient(TechType.PrecursorKey_Purple, 1),
#endif
                    new Ingredient(TechType.Lubricant, 1)
                }
            };
        }
#if SN1
        public override GameObject GetGameObject()
        {
            if(prefab == null)
            {
                var trashPrefab = CraftData.GetPrefabForTechType(TechType.Trashcans);
                prefab = GameObject.Instantiate(trashPrefab);
                GameObject.Destroy(prefab.GetComponent<Trashcan>());
                prefab.AddComponent<ItemResearchStation>();
                prefab.SetActive(false);
            }
            var obj = GameObject.Instantiate(prefab);
            obj.SetActive(true);
            return obj;
        }
#endif
        public override IEnumerator GetGameObjectAsync(IOut<GameObject> gameObject)
        {
            if(prefab == null)
            {
                var task = CraftData.GetPrefabForTechTypeAsync(TechType.Trashcans);
                yield return task;
                var trashPrefab = task.GetResult();
                    
                prefab = GameObject.Instantiate(trashPrefab);
                GameObject.Destroy(prefab.GetComponent<Trashcan>());
                prefab.AddComponent<ItemResearchStation>();
                prefab.SetActive(false);
            }
            var obj = GameObject.Instantiate(prefab);
            obj.SetActive(true);
            gameObject.Set(obj);
        }
    }
}
