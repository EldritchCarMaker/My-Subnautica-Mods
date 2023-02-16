using SMLHelper.V2.Assets;
using SMLHelper.V2.Crafting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Snomod.Prefabs
{
    internal class AmogusBackpack : Equipable
    {
        public AmogusBackpack() : base("AmogusBackpack", "Amogus backpack", "The backpack of an amogus, may contain some abnormal items")
        {

        }
        private static GameObject prefab;

        public override EquipmentType EquipmentType => EquipmentType.Tank;

        protected override TechData GetBlueprintRecipe()
        {
            return new TechData()
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient(TechType.FiberMesh, 1),
                    new Ingredient(Amogus.TT, 1)
                }
            };
        }
        public override GameObject GetGameObject()
        {
            if (!prefab)
            {
                prefab = Amogus.bundle.LoadAsset<GameObject>("AmongUsBackpack_Prefab");//lee made it, blame the long name on him
                prefab.GetComponent<StorageContainer>().storageRoot.ClassId = "MogusBackpack";
                prefab.SetActive(false);
            }

            var obj = GameObject.Instantiate(prefab);
            return obj;
        }
        public override IEnumerator GetGameObjectAsync(IOut<GameObject> gameObject)
        {
            gameObject.Set(GetGameObject());
            yield return null;
        }
        public override CraftTree.Type FabricatorType => CraftTree.Type.Fabricator;
        public override string[] StepsToFabricatorTab => new[] { "Root" };
        protected override Atlas.Sprite GetItemSprite()
        {
            return new Atlas.Sprite(Amogus.bundle.LoadAsset<UnityEngine.Sprite>("amogusBackpackIcon"));
        }
    }
}
