using SMLHelper.V2.Assets;
using SMLHelper.V2.Crafting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WarpChip.Monobehaviours;

namespace WarpChip.Items
{
    internal class TelePingBeacon : Equipable
    {
        public static TechType ItemTechType { get; private set; }
        public TelePingBeacon() : base("telepingbeacon", "Teleping beacon", "ASDA")
        {
            OnFinishedPatching += () => ItemTechType = TechType;
        }

        public override EquipmentType EquipmentType => EquipmentType.Hand;

        protected override TechData GetBlueprintRecipe()
        {
            return new TechData() 
            { 
                craftAmount = 1, 
                Ingredients = new List<Ingredient>() 
                { 
                    new Ingredient(TechType.Beacon, 1), 
                    new Ingredient(TechType.PrecursorIonCrystal, 1) 
                } 
            };
        }
        protected override Atlas.Sprite GetItemSprite()
        {
            return SpriteManager.Get(TechType.Beacon);
        }
        public override IEnumerator GetGameObjectAsync(IOut<GameObject> obj)
        {
            var task = CraftData.GetPrefabForTechTypeAsync(TechType.Beacon);
            yield return task;
            var prefab = GameObject.Instantiate(task.GetResult());
            prefab.AddComponent<TelePingInstance>();
            obj.Set(prefab);
        }
    }
}
