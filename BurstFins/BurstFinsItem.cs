using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Sprite = Atlas.Sprite;
using SMLHelper.V2.Assets;
using SMLHelper.V2.Crafting;
using SMLHelper.V2.Utility;
using UnityEngine;

namespace BurstFins
{
    internal class BurstFinsItem : Equipable
    {
        public static TechType thisTechType;
        public static Sprite sprite = SpriteManager.Get(TechType.UltraGlideFins);
        public override string AssetsFolder => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");

        public BurstFinsItem() : base("BurstFinsItem", "Burst Fins", "Allows a short burst of speed before going on cooldown")
        {
            OnFinishedPatching += () =>
            {
                thisTechType = TechType;
            };
        }

        public override EquipmentType EquipmentType => EquipmentType.Foots;
        public override TechType RequiredForUnlock => TechType.UltraGlideFins;
        public override TechGroup GroupForPDA => TechGroup.Personal;
        public override TechCategory CategoryForPDA => TechCategory.Equipment;
        public override CraftTree.Type FabricatorType => CraftTree.Type.Fabricator;
        public override string[] StepsToFabricatorTab => new string[] { "Personal", "Equipment" };
        public override float CraftingTime => 3f;
        public override QuickSlotType QuickSlotType => QuickSlotType.Passive;
        protected override Sprite GetItemSprite()
        {
            var ChangedSprite = sprite;
            ChangedSprite.size = new Vector2(-ChangedSprite.size.x, ChangedSprite.size.y);
            return ChangedSprite;
        }

        protected override TechData GetBlueprintRecipe()
        {
            return new TechData()
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>(new Ingredient[]
                    {
                        new Ingredient(TechType.UraniniteCrystal, 3),
                        new Ingredient(TechType.ToyCar, 1),
                        new Ingredient(TechType.UltraGlideFins, 1)
                    }
                )
            };
        }

        public override GameObject GetGameObject()
        {
            var prefab = CraftData.GetPrefabForTechType(TechType.UltraGlideFins);
            var obj = GameObject.Instantiate(prefab);
            return obj;
        }
    }
}
