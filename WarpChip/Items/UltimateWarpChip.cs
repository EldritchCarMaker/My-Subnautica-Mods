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

namespace WarpChip
{
    internal class UltimateWarpChip : Equipable
    {
        public static TechType thisTechType;

        public string AssetsFolder => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");

        public UltimateWarpChip() : base("UltimateWarpChip", "Ultimate Warp Chip", "Allows infinite short range teleportation with no cooldown")
        {
            OnFinishedPatching += () =>
            {
                thisTechType = TechType;
            };
        }

        public override EquipmentType EquipmentType => EquipmentType.Chip;
        public override TechType RequiredForUnlock =>
#if SN
            TechType.HatchingEnzymes;
#else
            TechType.TeleportationTool;
#endif
        public override TechGroup GroupForPDA => TechGroup.Personal;
        public override TechCategory CategoryForPDA => TechCategory.Equipment;
        public override CraftTree.Type FabricatorType => CraftTree.Type.Fabricator;
        public override string[] StepsToFabricatorTab => new string[] { "Personal", "Equipment" };
        public override float CraftingTime => 3f;
        public override QuickSlotType QuickSlotType => QuickSlotType.Passive;
        protected override Sprite GetItemSprite()
        {
            return ImageUtils.LoadSpriteFromFile(Path.Combine(AssetsFolder, "WarpChipIcon.png"));
        }

        protected override RecipeData GetBlueprintRecipe()
        {
            return new RecipeData()
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>(new Ingredient[]
                    {
                        new Ingredient(TechType.PrecursorIonCrystal, 5),
                        new Ingredient(TechType.PrecursorIonPowerCell, 1),
#if SN
                        new Ingredient(TechType.HatchingEnzymes, 1),
#endif
                        new Ingredient(WarpChipItem.thisTechType, 1)
                    }
                )
            };
        }
#if SN1
        public override GameObject GetGameObject()
        {
            var prefab = CraftData.GetPrefabForTechType(TechType.MapRoomHUDChip);
            var obj = GameObject.Instantiate(prefab);
            return obj;
        }
#endif
        public override IEnumerator GetGameObjectAsync(IOut<GameObject> gameObject)
        {
            yield return CraftData.InstantiateFromPrefabAsync(TechType.MapRoomHUDChip, gameObject);
        }
    }
}
