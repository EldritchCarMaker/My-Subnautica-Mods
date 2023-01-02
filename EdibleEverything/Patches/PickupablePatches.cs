using HarmonyLib;
using UnityEngine;

namespace EdibleEverything.Patches
{
    [HarmonyPatch(typeof(Pickupable))]
    internal class PickupablePatches
    {
        [HarmonyPatch(nameof(Pickupable.Awake))]
        public static void Postfix(Pickupable __instance)
        {
            if (__instance == null) return;

            if(__instance.gameObject.TryGetComponent<Eatable>(out var oldEat))
            {
                if (__instance.GetTechType() == TechType.CookedHoleFish)
                {
                    GameObject.Destroy(oldEat);//I told you snom, have fun finding this!
                }
                return;//don't want to change a vanilla eatable item's values
            }


            var eatable = __instance.gameObject.AddComponent<Eatable>();

            var techType = __instance.GetTechType();

            if(QMod.SaveData.foodValues.TryGetValue(techType, out FoodValues value))
            {
                eatable.foodValue = value.Food;
                eatable.waterValue = value.Water;
#if BZ
                eatable.maxCharges = value.Charges;
                eatable.charges = value.Charges;

                eatable.healthValue = value.Health;
                eatable.coldMeterValue = value.Heat;
#endif
            }
            else
            {
                var food = Random.Range(-99, 100);
                var water = Random.Range(-99, 99);
                eatable.foodValue = food;
                eatable.waterValue = water;
#if BZ
                var charges = Random.Range(1, 5);
                eatable.maxCharges = charges;
                eatable.charges = charges;

                var heat = Random.Range(-100, 100);
                var health = Random.Range(-99, 99);
                eatable.healthValue = health;
                eatable.coldMeterValue = heat;

                QMod.SaveData.foodValues.Add(techType, new FoodValues() { Food = food, Water = water, Charges = charges,  Health = health, Heat = heat});
#else
                QMod.SaveData.foodValues.Add(techType, new FoodValues() { Food = food, Water = water });
#endif
            }
        }
    }
}
