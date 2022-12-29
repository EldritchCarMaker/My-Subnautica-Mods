using SMLHelper.V2.Assets;
using SMLHelper.V2.Crafting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
#if SN
using RecipeData = SMLHelper.V2.Crafting.TechData;
using Sprite = Atlas.Sprite;
#endif
using System.Threading.Tasks;
using SMLHelper.V2.Utility;
using UnityEngine;
using System.Collections;

namespace MiniatureVehicles
{
    internal class MiniatureVehicleModule : Equipable
    {
        public static TechType thisTechType;

        public override string AssetsFolder => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");

        public MiniatureVehicleModule() : base("MiniatureMountModule", "Miniature Vehicle Module", "Allows vehicles to shrink down")
        {
            OnFinishedPatching += () =>
            {
                thisTechType = TechType;
            };
        }

        public override EquipmentType EquipmentType => EquipmentType.VehicleModule; 
        public override TechType RequiredForUnlock =>
#if SN
            TechType.Seamoth;
#else
            TechType.SeaTruck;
#endif
        public override TechGroup GroupForPDA => TechGroup.VehicleUpgrades;
        public override TechCategory CategoryForPDA => TechCategory.VehicleUpgrades;
        public override CraftTree.Type FabricatorType => CraftTree.Type.SeamothUpgrades;
        public override string[] StepsToFabricatorTab => new string[] { "CommonModules" };
        public override float CraftingTime => 3f;
        public override QuickSlotType QuickSlotType => QuickSlotType.Toggleable;
        protected override Sprite GetItemSprite()
        {
            return ImageUtils.LoadSpriteFromFile(Path.Combine(AssetsFolder, "shrinkmoth.png"));
        }

        protected override RecipeData GetBlueprintRecipe()
        {
            return new RecipeData()
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>(new Ingredient[]
                    {
                        new Ingredient(TechType.Magnetite, 2),
                        new Ingredient(TechType.AdvancedWiringKit, 2),
                        new Ingredient(TechType.PrecursorIonCrystal, 1)

                    }
                )
            };
        }
#if SN1
        public override GameObject GetGameObject()
        {
            var prefab = CraftData.GetPrefabForTechType(TechType.VehicleArmorPlating);
            var obj = GameObject.Instantiate(prefab);
            return obj;
        }
#else
        public override IEnumerator GetGameObjectAsync(IOut<GameObject> gameObject)
        {
            yield return CraftData.InstantiateFromPrefabAsync(TechType.VehicleArmorPlating, gameObject);
        }
#endif
    }
}
