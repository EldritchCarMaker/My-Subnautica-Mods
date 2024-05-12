#if SN
using Sprite = Atlas.Sprite;
#if SN1
using RecipeData = SMLHelper.V2.Crafting.TechData;
using SMLHelper.V2.Assets;
using SMLHelper.V2.Crafting;
using SMLHelper.V2.Utility;
#else
using Nautilus.Crafting;
using static CraftData;
#endif
#endif
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using WarpChip.Monobehaviours;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;

namespace WarpChip.Items
{
    internal class TelePingBeacon
    {
        public static TechType ItemTechType { get; private set; }
        public static void Patch()
        {
            var customPrefab = new CustomPrefab("telepingbeacon", "Teleping beacon", "A beacon combined with precursor technology to allow for teleportation when combined with the warp chip", GetItemSprite());
            var template = new CloneTemplate(customPrefab.Info, TechType.Beacon);
            template.ModifyPrefab += (prefab) => prefab.AddComponent<TelePingBeaconInstance>();
            customPrefab.SetGameObject(template);

            customPrefab.SetRecipe(GetBlueprintRecipe()).WithFabricatorType(CraftTree.Type.Fabricator).StepsToFabricatorTab = new[] { "Machines" };
            customPrefab.SetUnlock(WarpChipItem.thisTechType).WithPdaGroupCategory(TechGroup.Personal, TechCategory.Equipment);
            customPrefab.SetEquipment(EquipmentType.Hand).WithQuickSlotType(QuickSlotType.Selectable);

            customPrefab.Register();
            ItemTechType = customPrefab.Info.TechType;
        }
        public static RecipeData GetBlueprintRecipe()
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
        public static Sprite GetItemSprite()
        {
            return SpriteManager.Get(TechType.Beacon);
        }
    }
}
