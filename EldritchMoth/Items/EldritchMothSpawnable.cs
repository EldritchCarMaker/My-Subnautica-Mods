using SMLHelper.V2.Assets;
using SMLHelper.V2.Crafting;
using SMLHelper.V2.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.IO;
using EldritchMoth.Mono;

namespace EldritchMoth.Items
{
    internal class EldritchMothSpawnable : Craftable
    {
        private static Texture2D _mainMothText = ImageUtils.LoadTextureFromFile(Path.Combine(Main.Assets, "mothText.png"));
        private static Texture2D _interiorMothText = ImageUtils.LoadTextureFromFile(Path.Combine(Main.Assets, "mothInteriorText.png"));
        private static Texture2D _powerCellMothText = ImageUtils.LoadTextureFromFile(Path.Combine(Main.Assets, "mothCellText.png"));

        public static TechType type { get; private set; }

        public EldritchMothSpawnable() : base("EldritchMoth", "Eldritch Moth", "A seamoth created by Eldritch Car Maker herself. She creates more than just cars")
        {
            OnFinishedPatching += () => type = TechType;
        }

        protected override TechData GetBlueprintRecipe()
        {
            var recipe = new List<Ingredient>();

            foreach (var ingredient in Main.Config.recipe) recipe.Add(new Ingredient(ingredient.Key, ingredient.Value));

            return new TechData()
            {
                craftAmount = 1,
                Ingredients = recipe
            };
        }

        public override float CraftingTime => 20;

        public override CraftTree.Type FabricatorType => CraftTree.Type.Constructor;

        public override string[] StepsToFabricatorTab => new[] { "Vehicles" };

        public override Vector2int SizeInInventory => new(3, 3);

        public override List<TechType> CompoundTechsForUnlock => Main.Config.blueprintRequirements;

        protected override Atlas.Sprite GetItemSprite()
        {
            return ImageUtils.LoadSpriteFromFile(Path.Combine(Main.Assets, "EldritchMothIcon.png"));
        }

#if SN1
        public override GameObject GetGameObject()
        {
            var obj = CraftData.InstantiateFromPrefab(TechType.Seamoth);
            ApplyTextures(obj);
            return obj;
        }
#else
        public override IEnumerator GetGameObjectAsync(IOut<GameObject> gameObject)
        {
            yield return CraftData.InstantiateFromPrefabAsync(TechType.Seamoth, gameObject);
            if (gameObject is TaskResult<GameObject> result) ApplyTextures(result.Get());
            yield break;
        }
#endif

        public static void ApplyTextures(GameObject moth)
        {
            moth.EnsureComponent<EldritchMothMono>();
            var model = moth.transform.Find("Model/Submersible_SeaMoth");

            //what the fuck are these names
            model.Find("Master_jnt/l_flap_jnt/Submersible_seaMoth_flap_R_geo").GetComponent<Renderer>().material.mainTexture = _mainMothText;

            var geo = model.Find("Submersible_seaMoth_geo");

            geo.transform.Find("seamoth_power_cell_slot_geo").GetComponent<Renderer>().material.mainTexture = _powerCellMothText;

            geo.transform.Find("Submersible_seaMoth_flap_R_geo").GetComponent<Renderer>().material.mainTexture = _mainMothText;
            geo.transform.Find("Submersible_SeaMoth_geo").GetComponent<Renderer>().material.mainTexture = _mainMothText;
            geo.transform.Find("Submersible_SeaMoth_hatch_geo").GetComponent<Renderer>().material.mainTexture = _mainMothText;
            geo.transform.Find("Submersible_seaMoth_upgrad_slots_01_door_geo").GetComponent<Renderer>().material.mainTexture = _mainMothText;

            var interior = geo.transform.Find("Submersible_SeaMoth_interior_geo").GetComponent<Renderer>();
            interior.material.mainTexture = _mainMothText;
            interior.materials[1].mainTexture = _interiorMothText;

            foreach(var light in moth.transform.Find("lights_parent").GetComponentsInChildren<Light>(true))
            {
                light.color = new Color(1, 0, 0.8f);
                light.range = 150;
                light.spotAngle = 80;
            }
        }
    }
}
