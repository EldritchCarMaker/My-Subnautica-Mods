#if !SN1
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Crafting;
using Nautilus.Utility;
using static CraftData;

namespace RechargerChips.Items;

internal class ChargerChipItem
{
    internal static TechType solarChipTechType;
    internal static TechType thermalChipTechType;
    internal static TechType comboChipTechType;
    public static readonly string AssetsFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");
    public static void Patch()
    {
        var solarChip = new CustomPrefab("SolarChargerChip", "Solar Charger Chip", "Uses solar energy to slowly charge batteries stored in the inventory", GetSprite("chip_solar"));
        solarChipTechType = solarChip.Info.TechType;

        solarChip.SetEquipment(EquipmentType.Chip);
        solarChip.SetPdaGroupCategory(TechGroup.Personal, TechCategory.Equipment);
        solarChip.SetGameObject(new CloneTemplate(solarChip.Info, TechType.MapRoomHUDChip));
        solarChip.SetRecipe(
            new RecipeData()
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>(new Ingredient[]
                    {
                        new Ingredient(TechType.ComputerChip, 1),
                        new Ingredient(TechType.Quartz, 2),
                        new Ingredient(TechType.Copper, 2),

                    }
                )
            }).WithStepsToFabricatorTab("Personal", "Equipment").WithFabricatorType(CraftTree.Type.Fabricator);
        solarChip.SetUnlock(TechType.SolarPanel);

        solarChip.Register();


        var thermalChip = new CustomPrefab("ThermalChargerChip", "Thermal Charger Chip", "Uses thermal energy to slowly charge batteries stored in the inventory", GetSprite("chip_thermal"));
        thermalChipTechType = thermalChip.Info.TechType;

        thermalChip.SetEquipment(EquipmentType.Chip);
        thermalChip.SetPdaGroupCategory(TechGroup.Personal, TechCategory.Equipment);
        thermalChip.SetGameObject(new CloneTemplate(thermalChip.Info, TechType.MapRoomHUDChip));
        thermalChip.SetRecipe(
            new RecipeData()
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>(new Ingredient[]
                    {
                        new Ingredient(TechType.ComputerChip, 1),
                        new Ingredient(TechType.Magnetite, 2),
                        new Ingredient(TechType.Aerogel, 1),
                    }
                )
            }).WithStepsToFabricatorTab("Personal", "Equipment").WithFabricatorType(CraftTree.Type.Fabricator);
        thermalChip.SetUnlock(TechType.ThermalPlant);

        thermalChip.Register();


        var combinedChip = new CustomPrefab("ComboChargerChip", "Combo Charger Chip", "Combines the solar and thermal charger chips", GetSprite("chip_solar_thermal"));
        comboChipTechType= combinedChip.Info.TechType;

        combinedChip.SetEquipment(EquipmentType.Chip);
        combinedChip.SetPdaGroupCategory(TechGroup.Personal, TechCategory.Equipment);
        combinedChip.SetGameObject(new CloneTemplate(combinedChip.Info, TechType.MapRoomHUDChip));
        combinedChip.SetRecipe(
            new RecipeData()
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>(new Ingredient[]
                    {
                        new Ingredient(solarChipTechType, 1),
                        new Ingredient(thermalChipTechType, 1),
                        new Ingredient(TechType.AdvancedWiringKit, 1)
                    }
                )
            }).WithStepsToFabricatorTab("Personal", "Equipment").WithFabricatorType(CraftTree.Type.Fabricator);
        combinedChip.SetUnlock(TechType.ThermalPlant);

        combinedChip.Register();
    }
    public static Atlas.Sprite GetSprite(string spriteName)//no file extension
    {
        return ImageUtils.LoadSpriteFromFile(Path.Combine(AssetsFolder, spriteName + ".png"));
    }
}
#endif