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

namespace WarpChip.Items
{
    internal class TelePingVehicleModule : Equipable
    {
        public static TechType techType;
        public TelePingVehicleModule() : base("TelePingVehicleModule", "Teleping Vehicle Module", "A vehicle upgrade module, used to modify the vehicle's internal beacon into a teleping beacon and allow instant teleportation through the use of a warp chip")
        {
            OnFinishedPatching += () => { techType = TechType; };

        }

        public override EquipmentType EquipmentType => EquipmentType.VehicleModule;

        protected override RecipeData GetBlueprintRecipe()
        {
            return new RecipeData(new List<Ingredient>() { new Ingredient(TelePingBeacon.ItemTechType, 1), new Ingredient(TechType.AdvancedWiringKit, 1) }) { craftAmount = 1 };
        }
        public override CraftTree.Type FabricatorType => CraftTree.Type.SeamothUpgrades;
        public override string[] StepsToFabricatorTab => new[] { "Root", "CommonModules" };
        public override QuickSlotType QuickSlotType => QuickSlotType.Passive;
        public override TechType RequiredForUnlock => TelePingBeacon.ItemTechType;
        
#if SN1
        public override GameObject GetGameObject()
        {
            return CraftData.InstantiateFromPrefab(TechType.VehicleStorageModule);
        }
#else
        public override IEnumerator GetGameObjectAsync(IOut<GameObject> gameObject)
        {
            yield return CraftData.InstantiateFromPrefabAsync(TechType.VehicleStorageModule, gameObject);
        }

        protected override Sprite GetItemSprite()
        {
            return SpriteManager.Get(TechType.VehicleStorageModule);
        }
#endif
    }
}
