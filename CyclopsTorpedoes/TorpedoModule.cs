﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sprite = Atlas.Sprite;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using MoreCyclopsUpgrades.API.Upgrades;

#if SN1
using SMLHelper.V2.Assets;
using SMLHelper.V2.Crafting;
using SMLHelper.V2.Utility;
#else
using Nautilus.Utility;
using Nautilus.Crafting;
using static CraftData;
#endif

namespace CyclopsTorpedoes
{
    internal class TorpedoModule : CyclopsUpgrade/*Equipable*/
    {
        public static TechType thisTechType;
        public override string AssetsFolder => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");
        public TorpedoModule() : base("CyclopsTorpedoModule", "Cyclops Torpedo Module", "Allows the cyclops to launch torpedoes aiming in the direction of the currently active camera")
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


        public Sprite GetItemSprite()
        {
            return ImageUtils.LoadSpriteFromFile(Path.Combine(AssetsFolder, "cyclops_torpedo_module.png"));
        }

        protected override RecipeData GetBlueprintRecipe()
        {
            return new RecipeData()
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
    }
}
