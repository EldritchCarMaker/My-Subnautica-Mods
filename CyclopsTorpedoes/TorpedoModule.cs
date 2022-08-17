using SMLHelper.V2.Assets;
using SMLHelper.V2.Crafting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sprite = Atlas.Sprite;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using SMLHelper.V2.Utility;
using MoreCyclopsUpgrades.API.Upgrades;

namespace CyclopsTorpedoes
{
    internal class TorpedoModule : CyclopsUpgrade
    {
        public static TechType thisTechType;
        public override string AssetsFolder => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");
        public TorpedoModule() : base("TorpedoModule", "Cyclops Torpedo Module", "Allows the cyclops to launch torpedoes aiming in the direction of the currently active camera")
        {
            OnFinishedPatching += () =>
            {
                thisTechType = TechType;
            };
        }
        public override TechType RequiredForUnlock => TechType.Cyclops;
        public override CraftTree.Type FabricatorType => CraftTree.Type.CyclopsFabricator;
        public override string[] StepsToFabricatorTab => new string[] { };
        public override float CraftingTime => 3f;
        public override QuickSlotType QuickSlotType => QuickSlotType.Passive;
        protected override Sprite GetItemSprite()
        {
            return ImageUtils.LoadSpriteFromFile(Path.Combine(AssetsFolder, "chip_solar.png"));
        }

        protected override TechData GetBlueprintRecipe()
        {
            return new TechData()
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>(new Ingredient[]
                    {
                        new Ingredient(TechType.SeamothTorpedoModule, 1),
                        new Ingredient(TechType.WiringKit, 1)
                    }
                )
            };
        }

        public override GameObject GetGameObject()
        {
            var prefab = CraftData.GetPrefabForTechType(TechType.CyclopsDecoyModule);
            var obj = GameObject.Instantiate(prefab);
            return obj;
        }
    }
}
