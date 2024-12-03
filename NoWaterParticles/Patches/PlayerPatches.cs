using HarmonyLib;
using System;
using System.Reflection;
using UnityEngine;
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
            if (!QMod.Config.modEnabled) return;

            __instance.transform.Find("camPivot/camRoot/camOffset/pdaCamPivot/SpawnPlayerFX/PlayerFX(Clone)").gameObject.SetActive(false);

            __instance.transform.Find("body/player_view/export_skeleton/head_rig/neck/chest/clav_L/clav_L_aim/shoulder_L/elbow_L/hand_L/hand_L_midl_base").gameObject.SetActive(false);

            __instance.transform.Find("body/player_view/export_skeleton/head_rig/neck/chest/clav_R/clav_R_aim/shoulder_R/elbow_R/hand_R/hand_R_midl_base").gameObject.SetActive(false);
        }
    }
}
