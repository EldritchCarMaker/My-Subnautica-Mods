using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FasterSnowFoxCharging;
using HarmonyLib;
using UnityEngine;
using Logger = QModManager.Utility.Logger;
using SMLHelper.V2.Assets;

namespace SnowFoxQuantumLocker
{
    internal class SnowFoxQuantumLocker
    {
        public static readonly Dictionary<Hoverbike, StorageContainer> StorageContainers = new();

        [HarmonyPatch(typeof(Hoverbike))]
        [HarmonyPatch("Update")]
        internal class PatchHoverbikeUpdate
        {
            [HarmonyPostfix]

            public static void Postfix(Hoverbike __instance)
            {
                KeyCode summonKey = QMod.Config.LockerKey; //still not working, just set it to "C" at default for now and never bothered looking into fixing the config
                if (!Player.main.inHovercraft || __instance != Player.main.GetComponentInParent<Hoverbike>() ||
                !StorageContainers.TryGetValue(__instance, out StorageContainer storageContainer) ||
                storageContainer == null) return;
            
                if (GameInput.GetKeyDown((KeyCode)summonKey) && __instance.GetPilotingCraft())
                {
                    storageContainer.Open(__instance.transform);
                }
            }
        }   
        [HarmonyPatch(typeof (Hoverbike), nameof(Hoverbike.Awake))]
        internal class PatchHoverbikeAwake
        {
            [HarmonyPostfix]
            public static void Postfix(Hoverbike __instance)
            {
                StorageContainers[__instance] = __instance.transform.Find("Deployed")?.Find("ToggleLights")?.GetComponent<StorageContainer>();
                if(QMod.Config.LockerType.Equals("Quantum"))
                {
                    
                }
                else if(QMod.Config.LockerType.Equals("Snowfox"))
                {
                    //haven't done this yet. not entirely sure how to
                    //__instance.gameObject.AddComponent<SnowfoxSharedStorage>();
                }
            }
        }
    }
}