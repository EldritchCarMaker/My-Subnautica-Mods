using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Crafting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static CraftData;

namespace Snomod.Prefabs
{
    internal class AmogusWand
    {
        internal static void Patch()
        {
            var sprite = Amogus.bundle.LoadAsset<UnityEngine.Sprite>("amogusWandIconPink");

            var prefab = new CustomPrefab("AmogusWand", "Amogus wand", "What good mod doesn't have magic in it?", sprite);

            prefab.SetEquipment(EquipmentType.Hand).WithQuickSlotType(QuickSlotType.Selectable);

            prefab.SetGameObject(GetGameObject);

            prefab.SetRecipe(GetBlueprintRecipe()).WithStepsToFabricatorTab(new[] { "Mogus" }).WithFabricatorType(CraftTree.Type.Fabricator);

            prefab.SetUnlock(Amogus.TT);

            prefab.Register();
            TT = prefab.Info.TechType;
        }
        public static TechType TT { get; private set; }

        public static RecipeData GetBlueprintRecipe()
        {
            return new RecipeData()
            {
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient(TechType.Silicone, 2),
                    new Ingredient(Amogus.TT, 4)
                },
                craftAmount = 1
            };
        }

        public static IEnumerator GetGameObject(IOut<GameObject> @out)
        {
            yield return null;
            var prefab = Amogus.bundle.LoadAsset<GameObject>("AmogusWand");
            prefab.SetActive(true);

            var obj = GameObject.Instantiate(prefab);
            @out.Set(obj);
        }
    }
}
