using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            }
            else
            {
                var food = new System.Random().Next(0, 100);
                var water = new System.Random().Next(0, 99);
                eatable.foodValue = food;
                eatable.waterValue = water;

                QMod.SaveData.foodValues.Add(techType, new FoodValues() { Food = food, Water = water });
            }
        }
    }
}
