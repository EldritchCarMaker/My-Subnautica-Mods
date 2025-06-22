using System.Collections;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Illuminautica.ColorOverrides;
using Illuminautica.Interop;
using UnityEngine;

namespace Illuminautica;

[BepInPlugin("EldritchCarMaker.Illuminautica", "Illuminautica", "1.0")]
internal class Plugin : BaseUnityPlugin
{
    internal static ManualLogSource logger;


#pragma warning disable IDE0051 // Remove unused private members (It's not unused)
    private void Awake()
    {
        logger = Logger;

        InteropManager.SetUpManager();

        gameObject.AddComponent<ColorManager>();

        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

        ColorManager.instance.AddNewColorOverride(new BiomeColorOverride(3));
        ColorManager.instance.AddNewColorOverride(new RandomColorOverride(1, 1));
    }
}
