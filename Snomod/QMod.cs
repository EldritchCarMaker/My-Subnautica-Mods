using System.Reflection;
using HarmonyLib;
#if !SN2
using QModManager.API.ModLoading;
using Logger = QModManager.Utility.Logger;
#else
using BepInEx;
using BepInEx.Logging;
#endif
using UnityEngine;
using SMLHelper.V2.Json;
using SMLHelper.V2.Options.Attributes;
using SMLHelper.V2.Handlers;
using System.IO;
using Snomod.Prefabs;
using Snomod.MonoBehaviours;

namespace Snomod
{
    [BepInPlugin("EldritchCarMaker.Snomod", "Snonnod", "1.0.2")]
    public class QMod : BaseUnityPlugin
    {
        internal static AmogusBackpack backpack;
        public void Awake()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var CyclopsLockers = ($"EldritchCarMaker_{assembly.GetName().Name}");
            Logger.LogInfo($"Patching {CyclopsLockers}");
            Harmony harmony = new Harmony(CyclopsLockers);
            harmony.PatchAll(assembly);

            new Amogus().Patch();
            new AmogusKnife().Patch();
            new AmogusWand().Patch();
            backpack = new AmogusBackpack();
            backpack.Patch();

            Logger.LogInfo("Patched successfully!");
        }
        public void Update()
        {
            if(MogusBackpackEquipListener.instance && !Cursor.visible && Input.GetKeyDown(KeyCode.R))
            {
                MogusBackpackEquipListener.instance.Open();
            }
        }
    }
}
