using HarmonyLib;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Crafting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Audio;
using UnityEngine;
using static CraftData;

namespace Snomod.Prefabs
{
    internal static class AmogusKnife
    {
        internal static void Patch()
        {
            var sprite = Amogus.bundle.LoadAsset<UnityEngine.Sprite>("amogusKnifeIcon");

            var prefab = new CustomPrefab("AmogusKnife", "AMoguas", "Sus knife", sprite);

            prefab.SetEquipment(EquipmentType.Hand).WithQuickSlotType(QuickSlotType.Selectable);

            prefab.SetGameObject(GetGameObject);

            prefab.SetRecipe(GetBlueprintRecipe()).WithStepsToFabricatorTab(new[] { "Root" }).WithFabricatorType(CraftTree.Type.Fabricator);

            prefab.Register();
            TT = prefab.Info.TechType;
        }
        public static TechType TT { get; private set; }

        public static RecipeData GetBlueprintRecipe()
        {
            return new RecipeData() 
            { 
                Ingredients = new List<Ingredient>() 
                { 
                    new Ingredient(TechType.Titanium, 1)
                }, 
                craftAmount = 1
            };
        }

        public static GameObject GetGameObject()
        {
            var prefab = Amogus.bundle.LoadAsset<GameObject>("AmogusKnife");
            prefab.SetActive(true);

            var obj = GameObject.Instantiate(prefab);
            return obj;
        }

        
        private static class Test21
        { 
            [HarmonyPatch(typeof(uGUI_CraftingMenu), nameof(uGUI_CraftingMenu.IsGrid))] 
            private static void Postfix(uGUI_CraftingMenu.Node node, ref bool __result)
            { 
                __result = ShouldGrid(node);
            }

            private static bool ShouldGrid(uGUI_CraftingMenu.Node node)
            {
                var craftings = 0;
                var tabs = 0;

                foreach (var child in node)
                {
                    if (child.action == TreeAction.Expand) tabs++;
                    else if (child.action == TreeAction.Craft) craftings++;
                }

                return craftings > tabs;
            }

            [HarmonyPatch(typeof(uGUI_CraftingMenu), nameof(uGUI_CraftingMenu.Collapse))] 
            private static void Postfix(uGUI_CraftingMenu.Node parent)
            {
                if (parent == null) return;

                if (parent.action != TreeAction.Craft) return;

                parent.icon.SetActive(false);
            }
        }
    }
}
