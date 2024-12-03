using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
#if SN1
using RecipeData = SMLHelper.V2.Crafting.TechData;
using SMLHelper.V2.Utility;
using SMLHelper.V2.Assets;
using SMLHelper.V2.Crafting;
#else
using Nautilus.Assets;
using Nautilus.Crafting;
#endif
using System.Threading.Tasks;
using Sprite = Atlas.Sprite;
using UnityEngine;
using System.Collections;
using Nautilus.Utility;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Assets.Gadgets;
using static CraftData;

namespace MiniatureVehicles;

#if SN1
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
    public override GameObject GetGameObject()
    {
        var prefab = CraftData.GetPrefabForTechType(TechType.VehicleArmorPlating);
        var obj = GameObject.Instantiate(prefab);
        return obj;
    }
    public override IEnumerator GetGameObjectAsync(IOut<GameObject> gameObject)
    {
        yield return CraftData.InstantiateFromPrefabAsync(TechType.VehicleArmorPlating, gameObject);
    }
}
#else
internal class MiniatureVehicleModule
{
    public static TechType thisTechType;
    public void Patch()
    {
        var sprite = ImageUtils.LoadSpriteFromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets", "shrinkmoth.png"));
        var customPrefab = new CustomPrefab("MiniatureMountModule", "Miniature Vehicle Module", "Allows vehicles to shrink down", sprite);

        customPrefab.SetGameObject(new CloneTemplate(customPrefab.Info, TechType.VehicleArmorPlating));
        customPrefab.SetEquipment(EquipmentType.VehicleModule).QuickSlotType = QuickSlotType.Toggleable;
        customPrefab.SetRecipe(GetBlueprintRecipe()).WithFabricatorType(CraftTree.Type.SeamothUpgrades).WithStepsToFabricatorTab("CommonModules").WithCraftingTime(5);
        customPrefab.SetUnlock(TechType.Seamoth);
        customPrefab.SetPdaGroupCategory(TechGroup.VehicleUpgrades, TechCategory.VehicleUpgrades);

        thisTechType = customPrefab.Info.TechType;

        customPrefab.Register();
    }
    protected static RecipeData GetBlueprintRecipe()
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
}
#endif
