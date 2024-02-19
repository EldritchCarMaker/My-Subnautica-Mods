using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
#if SN
using Sprite = Atlas.Sprite;
#if SN1
using RecipeData = SMLHelper.V2.Crafting.TechData;
using SMLHelper.V2.Assets;
using SMLHelper.V2.Crafting;
using SMLHelper.V2.Utility;
#else
using Nautilus.Crafting;
using Nautilus.Utility;
using EquippableItemIcons.API.SecretSMLNautilusAPIDontTouch;
using static CraftData;
#endif
#endif
using UnityEngine;
using ArmorSuit.Items;
using System.Collections;

namespace ArmorSuit
{
    internal class ArmorSuitItem : Equipable
    {
        public static TechType thisTechType;
        public static Sprite sprite = SpriteManager.Get(TechType.UltraGlideFins);

        public ArmorSuitItem() : base("ArmorSuitItem", "Armor Suit", "A high tech adaptive suit which gives high damage reduction to a specific damage type")
        {
            OnFinishedPatching += () =>
            {
                thisTechType = TechType;
            };
        }

        public override EquipmentType EquipmentType => EquipmentType.Body;
        
        public override TechType RequiredForUnlock => TechType.ReinforcedDiveSuit;
        
        public override TechGroup GroupForPDA => TechGroup.Personal;
        
        public override TechCategory CategoryForPDA => TechCategory.Equipment;
        
        public override CraftTree.Type FabricatorType => CraftTree.Type.Fabricator;
        
        public override string[] StepsToFabricatorTab => new string[] { "Personal", "Equipment" };
        
        public override float CraftingTime => 3f;

        public override Vector2int SizeInInventory => new Vector2int(2, 2);

        protected override Sprite GetItemSprite()
        {
            return ImageUtils.LoadSpriteFromFile(Path.Combine(ArmorSuitMono.AssetsFolder, "ArmorSuit.png"));
        }

        protected override RecipeData GetBlueprintRecipe()
        {
            return new RecipeData()
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>(new Ingredient[]
                    {
                        new Ingredient(IonFiber.TType, 2),
                        new Ingredient(TechType.ReinforcedGloves, 1),
                        new Ingredient(TechType.ReinforcedDiveSuit, 1)
                    }
                ),
                LinkedItems = new List<TechType>() { ArmorGlovesItem.techType }
            };
        }
#if SN1
        public override GameObject GetGameObject()
        {
            var prefab = CraftData.GetPrefabForTechType(TechType.ReinforcedDiveSuit);
            var obj = GameObject.Instantiate(prefab);
            return obj;
        }
#endif
        public override IEnumerator GetGameObjectAsync(IOut<GameObject> gameObject)
        {
            var task = CraftData.GetPrefabForTechTypeAsync(TechType.ReinforcedDiveSuit);
            yield return task;
            gameObject.Set(task.GetResult());
        }
    }
}
