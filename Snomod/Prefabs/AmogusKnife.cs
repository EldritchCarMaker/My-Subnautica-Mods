using SMLHelper.V2.Assets;
using SMLHelper.V2.Crafting;
using SMLHelper.V2.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Audio;
using UnityEngine;

namespace Snomod.Prefabs
{
    internal class AmogusKnife : Equipable
    {
        public static TechType TT { get; private set; }
        private static GameObject prefab;
        public AmogusKnife() : base("AmogusKnife", "AMoguas", "Sus knife")
        {
            OnFinishedPatching += () => TT = TechType;
        }

        public override EquipmentType EquipmentType => EquipmentType.Hand;

        protected override TechData GetBlueprintRecipe()
        {
            return new TechData() 
            { 
                Ingredients = new List<Ingredient>() 
                { 
                    new Ingredient(TechType.Knife, 1), 
                    new Ingredient(Amogus.TT, 1)
                }, 
                craftAmount = 1
            };
        }

        public override GameObject GetGameObject()
        {
            if (!prefab)
            {
                prefab = Amogus.bundle.LoadAsset<GameObject>("AmogusKnife");
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
        public override QuickSlotType QuickSlotType => QuickSlotType.Selectable;
        public override string[] StepsToFabricatorTab => new[] { "Root" };
        protected override Atlas.Sprite GetItemSprite()
        {
            return new Atlas.Sprite(Amogus.bundle.LoadAsset<UnityEngine.Sprite>("amogusKnifeIcon"));
        }
    }
}
