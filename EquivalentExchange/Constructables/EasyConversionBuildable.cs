using SMLHelper.V2.Assets;
using SMLHelper.V2.Crafting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using EquivalentExchange.Monobehaviours;

namespace EquivalentExchange.Constructables
{
    internal class EasyConversionBuildable : Buildable
    {
        public EasyConversionBuildable() : base("EasyConversionBuildable", "Easy Conversion Antenna", "While within range of this antenna, will allow for automatic conversion of ECM to items as required for fabrication")
        {

        }

        protected override TechData GetBlueprintRecipe()
        {
            return new TechData()
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
        public override TechGroup GroupForPDA { get; } = TechGroup.InteriorPieces;
        public override TechCategory CategoryForPDA { get; } = TechCategory.InteriorPiece;
        public override TechType RequiredForUnlock { get; } = TechType.PrecursorIonCrystal;
        public override GameObject GetGameObject()
        {
            var thermalPlantPrefab = CraftData.GetPrefabForTechType(TechType.ThermalPlant);
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

            return obj;
        }
    }
}
