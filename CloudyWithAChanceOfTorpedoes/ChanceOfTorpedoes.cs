using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Utility;
using static CraftData;

namespace CloudyWithAChanceOfTorpedoes;

internal class ChanceOfTorpedoes
{
    public static TechType ModuleType { get; private set; }
    public static void ForecastWeather()
    {
        var module = new CustomPrefab("BitchGonRain", "Unlimited Torpedo Module", "Drains energy from the vehicle to fabricate torpedoes from a single cloned one in the bay", SpriteManager.Get(TechType.WhirlpoolTorpedo));

        module.SetGameObject(new CloneTemplate(module.Info, TechType.VehicleHullModule1));
        module.SetEquipment(EquipmentType.VehicleModule).WithQuickSlotType(QuickSlotType.Passive);
        module.SetRecipe(new(
            new Ingredient(TechType.ComputerChip, 1),
            new Ingredient(TechType.PrecursorIonCrystal, 1),
            new Ingredient(TechType.Titanium, 4),
            new Ingredient(TechType.Gold, 2)
            )).WithFabricatorType(CraftTree.Type.SeamothUpgrades).WithStepsToFabricatorTab("CommonModules");
        module.SetUnlock(TechType.SeamothTorpedoModule).WithPdaGroupCategory(TechGroup.VehicleUpgrades, TechCategory.VehicleUpgrades);

        module.Register();
        ModuleType = module.Info.TechType;
    }
}
