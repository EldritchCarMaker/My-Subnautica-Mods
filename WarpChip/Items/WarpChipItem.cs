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
using static CraftData;
#endif
#endif
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Assets;
using static Atlas;
using Nautilus.Assets.Gadgets;

namespace WarpChip
{
    internal class WarpChipItem
    {
        public static TechType thisTechType;

        public static string AssetsFolder => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");
        public static void Patch()
        {
            var customPrefab = new CustomPrefab("WarpChip", "Warp Chip", "Allows short range teleportation", GetItemSprite());
            customPrefab.SetGameObject(new CloneTemplate(customPrefab.Info, TechType.MapRoomHUDChip));

            customPrefab.SetRecipe(GetBlueprintRecipe()).WithFabricatorType(CraftTree.Type.Fabricator).StepsToFabricatorTab = new[] { "Personal", "Equipment" };
            var unlockType =
#if SN
                TechType.PrecursorPrisonIonGenerator;
#else
                TechType.PrecursorIonBattery;
#endif
            customPrefab.SetUnlock(unlockType).WithPdaGroupCategory(TechGroup.Personal, TechCategory.Equipment);
            customPrefab.SetEquipment(EquipmentType.Chip);

            customPrefab.Register();
            thisTechType = customPrefab.Info.TechType;
        }
        public static Sprite GetItemSprite()
        {
            return ImageUtils.LoadSpriteFromFile(Path.Combine(AssetsFolder, "WarpChipIcon.png"));
        }

        public static RecipeData GetBlueprintRecipe()
        {
            return new RecipeData()
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>(new Ingredient[]
                    {
                        new Ingredient(TechType.PrecursorIonCrystal, 2),
                        new Ingredient(TechType.ComputerChip, 1),
                        new Ingredient(TechType.PrecursorIonBattery, 1)
                    }
                )
            };
        }
    }
}
