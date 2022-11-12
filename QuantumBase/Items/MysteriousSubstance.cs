using QuantumBase.Mono;
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

namespace QuantumBase.Items
{
    internal class MysteriousSubstance : Craftable
    {
        public override string AssetsFolder => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");

        private TechData recipe;
        private int tier;
        public MysteriousSubstance(float charge, int tier, TechData recipe, string description = "An unknown substance with an unknown use. Takes the form of dry pellets") : base($"MysteriousSubstance{tier}", $"Mysterious Substance Tier {tier}", description)
        {
            this.tier = tier;
            this.recipe = recipe;
            OnFinishedPatching += () => EnergyGenerator.substanceChargeValues.Add(TechType, charge);
        }

        public override GameObject GetGameObject()
        {
            return CraftData.InstantiateFromPrefab(TechType.Snack1);
        }
        protected override TechData GetBlueprintRecipe()
        {
            return recipe;
        }
        protected override Atlas.Sprite GetItemSprite()
        {
            return ImageUtils.LoadSpriteFromFile(Path.Combine(AssetsFolder, $"MysteriousSubstanceIcon.png"));
        }
        public override TechCategory CategoryForPDA => TechCategory.AdvancedMaterials;
        public override CraftTree.Type FabricatorType => CraftTree.Type.Fabricator;
        public override TechGroup GroupForPDA => TechGroup.Resources;
        public override TechType RequiredForUnlock => QMod.substanceUnlockType;
        public override string[] StepsToFabricatorTab => new[] { "Resources", "AdvancedMaterials" };
    }
}
