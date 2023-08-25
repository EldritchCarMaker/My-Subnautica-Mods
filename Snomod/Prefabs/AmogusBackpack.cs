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
    internal class AmogusBackpack
    {
        internal static void Patch()
        {
            var sprite = Amogus.bundle.LoadAsset<UnityEngine.Sprite>("amogusBackpackIcon");

            var prefab = new CustomPrefab("AmogusBackpack", "Amogus backpack", "The backpack of an amogus, may contain some abnormal items", sprite);

            prefab.SetEquipment(EquipmentType.Tank);

            prefab.SetGameObject(GetGameObject);

            prefab.SetRecipe(GetBlueprintRecipe()).WithStepsToFabricatorTab(new[] { "Root" }).WithFabricatorType(CraftTree.Type.Fabricator);

            prefab.Register();
        }


        protected static RecipeData GetBlueprintRecipe()
        {
            return new RecipeData()
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient(TechType.FiberMesh, 1),
                    new Ingredient(Amogus.TT, 1)
                }
            };
        }
        public static GameObject GetGameObject()
        {
            var prefab = Amogus.bundle.LoadAsset<GameObject>("AmongUsBackpack_Prefab");//lee made it, blame the long name on him
            prefab.GetComponent<StorageContainer>().storageRoot.ClassId = "MogusBackpack";
            prefab.SetActive(true);

            var obj = GameObject.Instantiate(prefab);
            return obj;
        }
    }
}
