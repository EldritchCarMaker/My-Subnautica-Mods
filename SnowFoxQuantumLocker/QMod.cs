using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using HarmonyLib;
using QModManager.API.ModLoading;
using Logger = QModManager.Utility.Logger;
using SMLHelper.V2.Json;
using SMLHelper.V2.Options.Attributes;
using SMLHelper.V2.Handlers;
using UnityEngine;
using UWE;
using System.Collections;

namespace FasterSnowFoxCharging
{
    [QModCore]
    public static class QMod
    {
        
        internal static Config Config { get; } = OptionsPanelHandler.Main.RegisterModOptions<Config>();
        [QModPatch]
        public static void Patch()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var testMod = ($"Nagorogan_{assembly.GetName().Name}");
            Logger.Log(Logger.Level.Info, $"Patching {testMod}");
            Harmony harmony = new Harmony(testMod);
            harmony.PatchAll(assembly);
            Logger.Log(Logger.Level.Info, "Patched successfully!");
            CoroutineHost.StartCoroutine(ModifyHoverBikePrefab());
        }
        public static IEnumerator ModifyHoverBikePrefab()
        {
            CoroutineTask<GameObject> request = CraftData.GetPrefabForTechTypeAsync(TechType.Hoverbike, false);
            yield return request;

            var prefab = request.GetResult();

            var coi = prefab.transform.Find("Deployed")?.Find("ToggleLights")?.GetComponent<ChildObjectIdentifier>();

            if (coi)
            {
                var storageContainer = coi.gameObject.EnsureComponent<StorageContainer>();
                storageContainer.prefabRoot = prefab;
                storageContainer.storageRoot = coi;

                storageContainer.width = 5;
                storageContainer.height = 5;
                storageContainer.storageLabel = "Snow Fox Storage";
            }
            else
            {
                Logger.Log(Logger.Level.Error, $"Failed to Find Hoverbike's ToggleLights COI. Unable to attach storage!");
            }
        }
    }
    [Menu("SnowFox Quantum Locker")]
    public class Config : ConfigFile
    {
        [Keybind("Quantum Locker Keybind", Tooltip = "When on the snowfox, press this key to open the built in quantum locker")]
        public KeyCode LockerKey = KeyCode.C;
        [Choice("Locker Type", new[] {"Standard", "Quantum", "Snowfox"}, Tooltip = "Decides what type of locker the snowfox will have. Standard is a standard locker, Quantum is a quantum locker that shares inventory with all other quantum lockers, Snowfox is a locker that shares inventory with all other snowfoxes")]
        public string LockerType = "Standard";
    }
}