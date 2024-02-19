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
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace TimeControlSuit
{
    internal class TimeSuitItem : Equipable
    {
        public static TechType thisTechType;
        public  string AssetsFolder => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");

        public TimeSuitItem() : base("TimeSuitItem", "Time Suit", "suit that allows use of a short lasting, but quick charging, personal Time")
        {
            OnFinishedPatching += () =>
            {
                thisTechType = TechType;
            };
        }

        public override EquipmentType EquipmentType => EquipmentType.Body;
        public override Vector2int SizeInInventory => new Vector2int(2, 2);
        public override TechType RequiredForUnlock =>
#if SN
            TechType.StasisRifle;
#else
            TechType.PrecursorIonBattery;
#endif
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

        protected override RecipeData GetBlueprintRecipe()
        {
            return new RecipeData()
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>(new Ingredient[]
                    {
#if SN
                        new Ingredient(TechType.StasisRifle, 1),
#else
                        new Ingredient(TechType.Magnetite, 2),
#endif
                        new Ingredient(TechType.FiberMesh, 2),
                        new Ingredient(TechType.PrecursorIonPowerCell, 1)
                    }
                )
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
