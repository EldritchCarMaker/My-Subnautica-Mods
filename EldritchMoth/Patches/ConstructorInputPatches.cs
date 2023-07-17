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
    internal class ConstructorInputPatches
    {
        [HarmonyPatch(nameof(ConstructorInput.Craft))]
        [HarmonyPostfix]
        public static void CraftPostfix(ConstructorInput __instance, TechType techType)
        {
            Color color = new(1, 0, 0.8f);

            if (techType == EldritchMothSpawnable.type)
            {
                foreach (GameObject gameObject in __instance.constructor.buildBots)
                {
                    LineRenderer lineRenderer = gameObject.GetComponent<ConstructorBuildBot>().lineRenderer;
                    lineRenderer.startColor = color;
                    lineRenderer.endColor = color;
                }
            }
        }
    }
}
