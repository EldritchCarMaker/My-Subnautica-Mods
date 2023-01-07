using System.Reflection;
using HarmonyLib;
#if !SN2
using QModManager.API.ModLoading;
using Logger = QModManager.Utility.Logger;
#else
using BepInEx;
#endif
using UnityEngine;
using SMLHelper.V2.Json;
using SMLHelper.V2.Options.Attributes;
using SMLHelper.V2.Handlers;
using System.IO;
using SMLHelper.V2.Json.Attributes;
using System.Collections.Generic;
using AutoStorageTransferCompatibility.Patches;

namespace AutoStorageTransferCompatibility
{
#if !SN2
    [QModCore]
    public static class QMod
    {
#else
    [BepInPlugin("EldritchCarMaker.AutoStorageTransferCompatibility", "Auto Storage Transfer Compatibility", "1.0.1")]
    public class QMod : BaseUnityPlugin
    {
#endif
#if !SN2
        [QModPatch]
        public static void Patch()
        {
#else
        public void Awake()
        {
#endif
            var assembly = Assembly.GetExecutingAssembly();
            var name = ($"EldritchCarMaker_{assembly.GetName().Name}");
#if !SN2
            Logger.Log(Logger.Level.Info, $"Patching {name}");
#else
            Logger.LogInfo($"Patching {name}");
#endif
            Harmony harmony = new Harmony(name);
            harmony.PatchAll(assembly);

#if SN1     //none of these are updated to 2.0, or ported to below zero yet
            if(QModManager.API.QModServices.Main.ModPresent("CyclopsBioReactor"))
                CyclopsBioReactorPatches.PatchCyclopsBioReactor(harmony);
            
            if(QModManager.API.QModServices.Main.ModPresent("CyclopsNuclearReactor"))
                CyclopsNukeReactorPatches.PatchCyclopsNukeReactor(harmony);
            
            if(QModManager.API.QModServices.Main.ModPresent("IonCubeGenerator"))
                ioncubegeneratorPatches.PatchCubeGen(harmony);

            if (QModManager.API.QModServices.Main.ModPresent("FCSProductionSolutions"))
                DeepDrillerHeavyDutyPatches.PatchDrill(harmony);

            if (QModManager.API.QModServices.Main.ModPresent("FCSProductionSolutions"))
                DeepDrillerLightDutyPatches.PatchDrill(harmony);

            if (QModManager.API.QModServices.Main.ModPresent("FCSAlterraHub"))
                AlterraHubDepotPatches.PatchDepot(harmony);
#endif

            //alterra gen

#if !SN2
            Logger.Log(Logger.Level.Info, "Patched successfully!");
#else
            Logger.LogInfo("Patched successfully!");
#endif
        }
    }
}
