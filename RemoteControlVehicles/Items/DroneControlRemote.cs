using RemoteControlVehicles.Monobehaviours;
using SMLHelper.V2.Assets;
using SMLHelper.V2.Crafting;
using SMLHelper.V2.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using RecipeData = SMLHelper.V2.Crafting.TechData;
using Sprite = Atlas.Sprite;

namespace RemoteControlVehicles.Items
{
    public class DroneControlRemote : Equipable
    {
        public static TechType thisTechType;

        public override string AssetsFolder => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");

        public DroneControlRemote() : base("DroneControlRemote", "Drone Control Remote", "Allows on-the-go remote control over vehicles and scanner room drones")
        {
            OnFinishedPatching += () =>
            {
                thisTechType = TechType;
            };
        }

        public override EquipmentType EquipmentType => EquipmentType.Hand;
        public override TechType RequiredForUnlock => TechType.BaseMapRoom;
        public override TechGroup GroupForPDA => TechGroup.Personal;
        public override TechCategory CategoryForPDA => TechCategory.Tools;
        public override CraftTree.Type FabricatorType => CraftTree.Type.Fabricator;
        public override string[] StepsToFabricatorTab => new[] { "Personal", "Tools" };
        public override float CraftingTime => 5f;
        public override QuickSlotType QuickSlotType => QuickSlotType.Selectable;
        protected override Sprite GetItemSprite()
        {
            return ImageUtils.LoadSpriteFromFile(Path.Combine(AssetsFolder, "HudChipRemoteControl.png"));
        }

        protected override RecipeData GetBlueprintRecipe()
        {
            return new RecipeData()
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>(new Ingredient[]
                    {
                        new Ingredient(TechType.Magnetite, 2),
                        new Ingredient(TechType.ComputerChip, 2),
                        new Ingredient(TechType.WiringKit, 1)

                    }
                )
            };
        }

        public override GameObject GetGameObject()
        {
            var prefab = CraftData.GetPrefabForTechType(TechType.AirBladder);
            var obj = GameObject.Instantiate(prefab);

            GameObject.Destroy(obj.GetComponent<AirBladder>());
            obj.AddComponent<DroneControl>();

            return obj;
        }
    }
}
