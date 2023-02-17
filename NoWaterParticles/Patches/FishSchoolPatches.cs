using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;

namespace NoWaterParticles.Patches
{
    [HarmonyPatch(typeof(VFXSchoolFish))]
    internal class FishSchoolPatches
    {
        [HarmonyPatch(nameof(VFXSchoolFish.Awake))]
        public static void Postfix(VFXSchoolFish __instance)
        {
            if (!QMod.Config.modEnabled) return;

            var identy = __instance.GetComponentInParent<PrefabIdentifier>();//fish schools generally have a prefab identifier on them
            if (identy)
            {
                GameObject.Destroy(identy.gameObject);//use it to make sure we target correct object
                return;
            }
            GameObject.Destroy(identy.gameObject);//otherwise just delete this object. it'll get rid of the renderer at least
        }
    }
}
