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
using Sprite = Atlas.Sprite;
using TechData = SMLHelper.V2.Crafting.TechData;

namespace CameraDroneRepairUpgrade.Items
{
    internal class MapRoomCameraRepairUpgrade : Craftable
    {
        public static TechType thisTechType;
        public override string AssetsFolder => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");

        public MapRoomCameraRepairUpgrade() : base("MapRoomCameraRepairUpgrade", "Drone Repair Upgrade", "Allows drones to repair objects they're looking at")
        {
            OnFinishedPatching += () =>
            {
                thisTechType = TechType;
            };
        }

        public override TechType RequiredForUnlock => TechType.BaseMapRoom;
        public override TechGroup GroupForPDA => TechGroup.Personal;
        public override TechCategory CategoryForPDA => TechCategory.Equipment;
        public override CraftTree.Type FabricatorType => CraftTree.Type.MapRoom;
        public override string[] StepsToFabricatorTab => new string[] {};
        public override float CraftingTime => 3f;
        protected override Sprite GetItemSprite()
        {
            return ImageUtils.LoadSpriteFromFile(Path.Combine(AssetsFolder, "RepairUpgrade.png"));
        }

        protected override TechData GetBlueprintRecipe()
        {
            return new TechData()
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>(new Ingredient[]
                    {
                        new Ingredient(TechType.Welder, 1),
                        new Ingredient(TechType.ComputerChip, 1),
                        new Ingredient(TechType.WiringKit, 1)
                    }
                )
            };
        }

        public override GameObject GetGameObject()
        {
            var prefab = CraftData.GetPrefabForTechType(TechType.MapRoomHUDChip);
            var obj = GameObject.Instantiate(prefab);
            return obj;
        }
    }
}
