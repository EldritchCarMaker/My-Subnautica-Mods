using System.Reflection;
using HarmonyLib;
using QModManager.API.ModLoading;
using Logger = QModManager.Utility.Logger;
using UnityEngine;
using System.Collections.Generic;
//using MoreCyclopsUpgrades.API;

namespace CyclopsTorpedoes
{
    [QModCore]
    public static class QMod
    {
        [QModPatch]
        public static void Patch()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var stingers = ($"Nagorogan_{assembly.GetName().Name}");
            Logger.Log(Logger.Level.Info, $"Patching {stingers}");
            Harmony harmony = new Harmony(stingers);
            harmony.PatchAll(assembly);

            Logger.Log(Logger.Level.Info, "Patched successfully!");
        }
    }
}