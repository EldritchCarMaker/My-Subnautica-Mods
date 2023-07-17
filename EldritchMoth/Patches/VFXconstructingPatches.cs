using EldritchMoth.Items;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace EldritchMoth.Patches
{
    [HarmonyPatch(typeof(VFXConstructing))]
    [HarmonyPatch("Construct")]
    public static class ConstructFXPatch
    {
        private static Color col = new(1, 0, 0.8f);

        public static void Prefix(VFXConstructing __instance)
        {
            if (CraftData.GetTechType(__instance.gameObject) == EldritchMothSpawnable.type)
                __instance.wireColor = col;
        }
        public static void Postfix(VFXConstructing __instance)
        {
            if(CraftData.GetTechType(__instance.gameObject) == EldritchMothSpawnable.type)
            {
                __instance.ghostMaterial = new Material(__instance.ghostMaterial);
                __instance.ghostOverlay.material = __instance.ghostMaterial;
                __instance.ghostMaterial.color = col;
            }
        }
    }
}
