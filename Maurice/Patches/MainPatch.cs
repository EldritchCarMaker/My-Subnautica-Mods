using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Maurice.Patches;

[HarmonyPatch(typeof(Creature))]
internal class MainPatch
{
    [HarmonyPatch(nameof(Creature.Start))]
    public static void Prefix(Creature __instance)
    {
        GameObject.DestroyImmediate(__instance.gameObject);
    }
}
