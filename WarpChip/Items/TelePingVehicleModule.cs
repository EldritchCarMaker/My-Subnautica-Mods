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

namespace WarpChip.Items
{
    internal class TelePingVehicleModule
    {
        public static TechType techType;
        public static void Patch()
        {
            var customPrefab = new CustomPrefab("TelePingVehicleModule", "Teleping Vehicle Module", "A vehicle upgrade module, used to modify the vehicle's internal beacon into a teleping beacon and allow instant teleportation through the use of a warp chip", GetItemSprite());
            customPrefab.SetGameObject(new CloneTemplate(customPrefab.Info, TechType.VehicleStorageModule));

            customPrefab.SetRecipe(GetBlueprintRecipe()).WithFabricatorType(CraftTree.Type.SeamothUpgrades).StepsToFabricatorTab = new[] { "Root", "CommonModules" };
            customPrefab.SetUnlock(TelePingBeacon.ItemTechType).WithPdaGroupCategory(TechGroup.Personal, TechCategory.Equipment);
            customPrefab.SetEquipment(EquipmentType.Chip).WithQuickSlotType(QuickSlotType.Passive);

            customPrefab.Register();
            techType = customPrefab.Info.TechType;
        }
        public static RecipeData GetBlueprintRecipe()
        {
            return new RecipeData(new List<Ingredient>() { new Ingredient(TelePingBeacon.ItemTechType, 1), new Ingredient(TechType.AdvancedWiringKit, 1) }) { craftAmount = 1 };
        }


        public static Sprite GetItemSprite()
        {
            return SpriteManager.Get(TechType.VehicleStorageModule);
        }
    }
}
