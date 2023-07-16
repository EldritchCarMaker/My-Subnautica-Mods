using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;

namespace LuckyBlock.Patches
{
    internal class RotAPatches
    {
        public static void Patch(Harmony harmony)
        {
            var target = AccessTools.Method(typeof(RotA.Mono.Cinematics.SunbeamGargController), "Start");
            var patch = typeof(RotAPatches).GetMethod(nameof(SunbeamGargPrefix));

            harmony.Patch(target, new(patch));
        }

        //the start method sets everything up, and we only want the garg jump not the whole sunbeam thing
        public static bool SunbeamGargPrefix(Component __instance)
        {
            if (__instance.TryGetComponent<SunbeamGargMarker>(out _)) return false;
            return true;
        }
    }
    public class SunbeamGargMarker : MonoBehaviour
    {

    }
}
