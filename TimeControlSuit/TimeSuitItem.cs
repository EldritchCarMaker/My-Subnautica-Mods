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

namespace TimeControlSuit
{
    internal class TimeSuitItem : Equipable
    {
        public static TechType thisTechType;
        public override string AssetsFolder => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");

        public TimeSuitItem() : base("TimeSuitItem", "Time Suit", "suit that allows use of a short lasting, but quick charging, personal Time")
        {
            OnFinishedPatching += () =>
            {
                thisTechType = TechType;
            };
        }

        public override EquipmentType EquipmentType => EquipmentType.Body;
        public override Vector2int SizeInInventory => new Vector2int(2, 2);
        public override TechType RequiredForUnlock => TechType.StasisRifle;
        public override TechGroup GroupForPDA => TechGroup.Personal;
        public override TechCategory CategoryForPDA => TechCategory.Equipment;
        public override CraftTree.Type FabricatorType => CraftTree.Type.Fabricator;
        public override string[] StepsToFabricatorTab => new string[] { "Personal", "Equipment" };
        public override float CraftingTime => 3f;
        public override QuickSlotType QuickSlotType => QuickSlotType.Passive;
        protected override Sprite GetItemSprite()
        {
            return ImageUtils.LoadSpriteFromFile(Path.Combine(AssetsFolder, "TimeSuitItem.png"));
        }

        protected override TechData GetBlueprintRecipe()
        {
            return new TechData()
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>(new Ingredient[]
                    {
                        new Ingredient(TechType.StasisRifle, 1),
                        new Ingredient(TechType.FiberMesh, 2),
                        new Ingredient(TechType.PrecursorIonPowerCell, 1)
                    }
                )
            };
        }

        public override GameObject GetGameObject()
        {
            var prefab = CraftData.GetPrefabForTechType(TechType.ReinforcedDiveSuit);
            var obj = GameObject.Instantiate(prefab);
            return obj;
        }
    }
}
