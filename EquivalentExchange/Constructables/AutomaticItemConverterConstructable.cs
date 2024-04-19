using EquivalentExchange.Monobehaviours;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Crafting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static CraftData;

#if SN
using Sprite = Atlas.Sprite;
#endif

namespace EquivalentExchange.Constructables
{
    internal class AutomaticItemConverterConstructable
    {
        public static void Patch()
        {
            var customPrefab = new CustomPrefab("AutomaticItemConverterConstructable", "Automatic Item Converter", "A small container that automatically, and constantly, converts ECM into a single, specified, item");

            var template = new CloneTemplate(customPrefab.Info, TechType.MedicalCabinet);
            template.ModifyPrefab += (prefb => prefb.EnsureComponent<AutomaticItemConverter>());
            customPrefab.SetGameObject(template);

            customPrefab.SetRecipe(GetBlueprintRecipe());
            customPrefab.SetPdaGroupCategory(TechGroup.InteriorModules, TechCategory.InteriorModule);
            customPrefab.SetUnlock(TechType.PrecursorIonCrystal);

            customPrefab.Register();
        }

        protected static RecipeData GetBlueprintRecipe()
        {
            return new RecipeData
            {
                Ingredients =
                {
                    new Ingredient(TechType.PrecursorIonCrystal, 1),
                    new Ingredient(TechType.Titanium, 1),
                    new Ingredient(TechType.Lubricant, 1)
                }
            };
        }
    }
}