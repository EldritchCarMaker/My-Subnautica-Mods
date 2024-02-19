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
using WarpChip.Monobehaviours;

namespace WarpChip.Items
{
    internal class TelePingBeacon : Equipable
    {
        public static TechType ItemTechType { get; private set; }
        public TelePingBeacon() : base("telepingbeacon", "Teleping beacon", "A beacon combined with precursor technology to allow for teleportation when combined with the warp chip")
        {
            OnFinishedPatching += () => ItemTechType = TechType;
        }
        public override CraftTree.Type FabricatorType => CraftTree.Type.Fabricator;
        public override string[] StepsToFabricatorTab => new[] { "Machines" };
        public override TechType RequiredForUnlock => WarpChipItem.thisTechType;
        public override QuickSlotType QuickSlotType => QuickSlotType.Selectable;
        public override EquipmentType EquipmentType => EquipmentType.Hand;

        protected override RecipeData GetBlueprintRecipe()
        {
            return new RecipeData() 
            { 
                craftAmount = 1, 
                Ingredients = new List<Ingredient>() 
                { 
                    new Ingredient(TechType.Beacon, 1), 
                    new Ingredient(TechType.PrecursorIonCrystal, 1) 
                } 
            };
        }
        protected override Sprite GetItemSprite()
        {
            return SpriteManager.Get(TechType.Beacon);
        }
        public override IEnumerator GetGameObjectAsync(IOut<GameObject> obj)
        {
            var task = CraftData.GetPrefabForTechTypeAsync(TechType.Beacon);
            yield return task;
            var prefab = GameObject.Instantiate(task.GetResult());
            prefab.AddComponent<TelePingBeaconInstance>();
            obj.Set(prefab);
        }
#if SN1
        public override GameObject GetGameObject()
        {
            var prefab = CraftData.InstantiateFromPrefab(TechType.Beacon);

            prefab.AddComponent<TelePingBeaconInstance>();
            return prefab;
        }
#endif
    }
}
