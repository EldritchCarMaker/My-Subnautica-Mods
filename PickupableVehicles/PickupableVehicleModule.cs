
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
#if SN
#if SN1
using SMLHelper.V2.Assets;
using SMLHelper.V2.Crafting;
using SMLHelper.V2.Utility;
using RecipeData = SMLHelper.V2.Crafting.TechData;
#else
using Nautilus.Crafting;
using Nautilus.Assets;
using Nautilus.Assets.PrefabTemplates;
#endif
using Sprite = Atlas.Sprite;
#endif
using System.Threading.Tasks;
using UnityEngine;
using System.Collections;
using Nautilus.Assets.Gadgets;
using static CraftData;
using Nautilus.Utility;

namespace PickupableVehicles
{
#if SN1
    internal class PickupableVehicleModule : Equipable
    {
        public static TechType thisTechType;

        public override string AssetsFolder => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");

        public PickupableVehicleModule() : base("PickupableVehicleModule", "Pickupable Vehicle Module", "Allows you to pick up vehicle by holding sprint")
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
        public override QuickSlotType QuickSlotType => QuickSlotType.Passive;
        protected override Sprite GetItemSprite()
        {
            return ImageUtils.LoadSpriteFromFile(Path.Combine(AssetsFolder, "seamoth_boxing_a.png"));
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
#else
    internal class PickupableVehicleModule
    {
        public static TechType thisTechType;
        public void Patch()
        {
            var AssetsFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");
            var customPrefab = new CustomPrefab("PickupableVehicleModule", "Pickupable Vehicle Module", "Allows you to pick up vehicle by holding sprint", ImageUtils.LoadSpriteFromFile(Path.Combine(AssetsFolder, "seamoth_boxing_a.png")));

            customPrefab.SetGameObject(new CloneTemplate(customPrefab.Info, TechType.VehicleArmorPlating));
            customPrefab.SetEquipment(EquipmentType.VehicleModule).QuickSlotType = QuickSlotType.Passive;
            customPrefab.SetRecipe(new RecipeData()
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>(new Ingredient[]
                    {
                        new Ingredient(TechType.Magnetite, 2),
                        new Ingredient(TechType.AdvancedWiringKit, 2),
                        new Ingredient(TechType.PrecursorIonCrystal, 1)

                    }
                )
            }).WithFabricatorType(CraftTree.Type.Fabricator).WithCraftingTime(3).WithStepsToFabricatorTab(new string[] { "CommonModules" });
            customPrefab.SetPdaGroupCategory(TechGroup.VehicleUpgrades, TechCategory.VehicleUpgrades);

            customPrefab.Register();
            thisTechType = customPrefab.Info.TechType;
        }
    }
#endif
}
