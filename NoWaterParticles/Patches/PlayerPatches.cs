using HarmonyLib;
using System;
using System.Reflection;
using UnityEngine;
using SMLHelper.V2.Utility;
using System.IO;
using UnityEngine.UI;
using UWE;
using System.Collections;

namespace NoWaterParticles
{
    [HarmonyPatch(typeof(Player))]
    internal class PlayerPatches 
    {
        [HarmonyPatch(nameof(Player.Start))]
        [HarmonyPostfix]
        private static void Postfix(Player __instance)
        {
            __instance.transform.Find("camPivot/camRoot/camOffset/pdaCamPivot/SpawnPlayerFX/PlayerFX(Clone)").gameObject.SetActive(false);
        }
    }
}
