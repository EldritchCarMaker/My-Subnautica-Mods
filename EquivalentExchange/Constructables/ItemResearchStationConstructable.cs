using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using EquivalentExchange.Monobehaviours;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Crafting;
using UnityEngine;
using static CraftData;

#if SN
using Sprite = Atlas.Sprite;
#endif

namespace EquivalentExchange.Constructables
{
    internal class ItemResearchStationConstructable
    {
        public static void Patch()
        {
            var customPrefab = new CustomPrefab("ItemResearchStationConstructable", "Item Research Station", "A station used to research items for the purposes of material exchange");

            customPrefab.SetGameObject(GetGameObjectAsync);

            customPrefab.SetRecipe(GetBlueprintRecipe());
            customPrefab.SetPdaGroupCategory(TechGroup.InteriorPieces, TechCategory.InteriorPiece);
            customPrefab.SetUnlock(TechType.PrecursorIonCrystal);

            customPrefab.Register();
        }

        protected static RecipeData GetBlueprintRecipe()
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
        public static IEnumerator GetGameObjectAsync(IOut<GameObject> gameObject)
        {
            var task = CraftData.GetPrefabForTechTypeAsync(TechType.Trashcans);
            yield return task;
            var trashPrefab = task.GetResult();

            var prefab = GameObject.Instantiate(trashPrefab);
            GameObject.Destroy(prefab.GetComponent<Trashcan>());
            prefab.AddComponent<ItemResearchStation>();

            gameObject.Set(prefab);
        }
    }
}
