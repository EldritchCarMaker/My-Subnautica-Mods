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
using SMLHelper.Json;
using SMLHelper.Options.Attributes;
using SMLHelper.Handlers;
using System.IO;
using Snomod.Prefabs;
using Snomod.MonoBehaviours;

namespace Snomod
{
    [BepInPlugin("EldritchCarMaker.Snomod", "Snonnod", "1.0.2")]
    public class QMod : BaseUnityPlugin
    {
        public void Awake()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var CyclopsLockers = ($"EldritchCarMaker_{assembly.GetName().Name}");
            Logger.LogInfo($"Patching {CyclopsLockers}");
            Harmony harmony = new Harmony(CyclopsLockers);
            harmony.PatchAll(assembly);

            Amogus.Patch();
            AmogusKnife.Patch();
            AmogusWand.Patch();
            AmogusBackpack.Patch();

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
