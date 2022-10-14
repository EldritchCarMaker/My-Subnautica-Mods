using System.Reflection;
using HarmonyLib;
using QModManager.API.ModLoading;
using Logger = QModManager.Utility.Logger;
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
    [QModCore]
    public static class QMod
    {
        [QModPatch]
        public static void Patch()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var name = ($"Nagorogan_{assembly.GetName().Name}");
            Logger.Log(Logger.Level.Info, $"Patching {name}");
            Harmony harmony = new Harmony(name);
            harmony.PatchAll(assembly);

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

            //alterra gen

            Logger.Log(Logger.Level.Info, "Patched successfully!");
        }
    }
}
