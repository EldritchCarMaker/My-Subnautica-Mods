using Nautilus.Assets;
using Nautilus.Crafting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using EquivalentExchange.Monobehaviours;
using System.Collections;
using Nautilus.Assets.Gadgets;

using Nautilus.Assets.PrefabTemplates;
using static CraftData;


#if SN
using Sprite = Atlas.Sprite;
#endif

namespace EquivalentExchange.Constructables
{
    internal class EasyConversionBuildable
    {
        public static void Patch()
        {
            var customPrefab = new CustomPrefab("EasyConversionBuildable", "Easy Conversion Antenna", "While within range of this antenna, will allow for automatic conversion of ECM to items as required for fabrication");

            customPrefab.SetGameObject(GetGameObjectAsync);

            customPrefab.SetRecipe(GetBlueprintRecipe());
            customPrefab.SetPdaGroupCategory(TechGroup.InteriorPieces, TechCategory.InteriorPiece);
            customPrefab.SetUnlock(TechType.PrecursorIonCrystal);

            customPrefab.Register();
        }
        protected static RecipeData GetBlueprintRecipe()
        {
            return new RecipeData()
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient(TechType.AluminumOxide, 2),
                    new Ingredient(TechType.ComputerChip, 1),
                    new Ingredient(TechType.PrecursorIonCrystal, 1),
                    new Ingredient(TechType.Magnetite, 3)
                }
            };
        }
#if SN1
        public override GameObject GetGameObject()
        {
            var thermalPlantPrefab = CraftData.GetPrefabForTechType(TechType.ThermalPlant);
#else
        public static IEnumerator GetGameObjectAsync(IOut<GameObject> gameObject)
        {
            var task = CraftData.GetPrefabForTechTypeAsync(TechType.ThermalPlant);
            yield return task;
            var thermalPlantPrefab = task.GetResult();
#endif
            var obj = GameObject.Instantiate(thermalPlantPrefab);

            GameObject.Destroy(obj.GetComponent<ThermalPlant>());
            GameObject.Destroy(obj.GetComponent<PowerFX>());
            GameObject.Destroy(obj.GetComponent<PowerFX>());//two components on here for some reason
            GameObject.Destroy(obj.GetComponent<PowerSystemPreview>());
            GameObject.Destroy(obj.GetComponent<PowerSource>());
            GameObject.Destroy(obj.GetComponent<PowerRelay>());

            //GameObject.Destroy(obj.transform.Find("UI/Canvas/temperatureBar"));

            obj.AddComponent<EasyConversionAntenna>();

            obj.transform.localScale = new Vector3(0.45f, 0.5f, 0.45f);

            var constructable = obj.GetComponent<Constructable>();
            constructable.allowedInBase = true;
            constructable.allowedInSub = true;
            constructable.allowedOnCeiling = false;
            constructable.allowedOnConstructables = true;
            constructable.allowedOnGround = true;
            constructable.allowedOnWall = false;
            constructable.allowedOutside = true;

#if SN1
            return obj;
#else 
            gameObject.Set(obj);
#endif
        }
    }
}
