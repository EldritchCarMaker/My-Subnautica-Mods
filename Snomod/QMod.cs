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
using System.IO;
using Snomod.Prefabs;
using Snomod.MonoBehaviours;
using Nautilus.Handlers;
using Snomod.Patches;

namespace Snomod
{
    [BepInPlugin("EldritchCarMaker.Snomod", "Snonnod", "1.0.2")]
    public class QMod : BaseUnityPlugin
    {
        public static new ManualLogSource Logger { get; private set; }
        public void Awake()
        {
            QMod.Logger = base.Logger;

            var assembly = Assembly.GetExecutingAssembly();
            var CyclopsLockers = ($"EldritchCarMaker_{assembly.GetName().Name}");
            Logger.LogInfo($"Patching {CyclopsLockers}");
            Harmony harmony = new Harmony(CyclopsLockers);
            harmony.PatchAll(assembly);

            CraftTreeHandler.AddTabNode(CraftTree.Type.Fabricator, "Mogus", "Mogus", Prefabs.Amogus.bundle.LoadAsset<UnityEngine.Sprite>("AmogusIconRed"));

            Amogus.Patch();
            AmogusKnife.Patch();
            AmogusWand.Patch();
            AmogusBackpack.Patch();

            if(BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.tobeyandkallie23.socksfor1modport"))
            {
                Logger.LogInfo("Old socks detected, cleaning stink");
                OldSocksCommissionCompatPatch.Patch();
            }

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
