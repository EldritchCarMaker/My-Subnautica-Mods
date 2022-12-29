using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UWE;

namespace MiniatureVehicles.Patches
{
    [HarmonyPatch(typeof(Vehicle))]
    internal class VehiclePatches
    {
        public const string SHRINK_SOUND_PATH = "event:/creature/warper/portal_close";
        public const string ENLARGE_SOUND_PATH = "event:/creature/warper/portal_open";
        public static float TeleportDuration = 0.25f;
        [HarmonyPatch(nameof(Vehicle.ToggleSlot), new[] { typeof(int), typeof(bool) })]//for some reason the upgradeModuleToggle method isn't called for the seamoth for me
                                                                                       //might be another mod fucking with it, don't know don't care
        public static void Postfix(Vehicle __instance, int slotID, bool state)
        {
            var tt = __instance.GetSlotBinding(slotID);

            if (tt == MiniatureVehicleModule.thisTechType)
            {
                if (state)
                {
                    __instance.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                    __instance.transform.position += 0.5f * __instance.transform.up;
                    Utils.PlayFMODAsset(GetFmodAsset(SHRINK_SOUND_PATH), __instance.transform.position);
                    CoroutineHost.StartCoroutine(TeleportFX(TeleportDuration));

                }
                else
                {
                    __instance.transform.localScale = new Vector3(1, 1, 1);
                    Utils.PlayFMODAsset(GetFmodAsset(ENLARGE_SOUND_PATH), __instance.transform.position);
                    CoroutineHost.StartCoroutine(TeleportFX(TeleportDuration));
                }
#if SN
                if (__instance is SeaMoth)
                {
                    //trail effects covered screen when shrunk
                    var trail = __instance.transform.Find("Model/xSeamothTrail");
                    if (trail != null) trail.gameObject.SetActive(!state);
                }
#endif
            }
        }
        public static FMODAsset GetFmodAsset(string audioPath)
        {
            FMODAsset asset = ScriptableObject.CreateInstance<FMODAsset>();
            asset.path = audioPath;
            return asset;
        }

        public static IEnumerator TeleportFX(float delay = 0.25f)
        {
            TeleportScreenFXController fxController = MainCamera.camera.GetComponent<TeleportScreenFXController>();
#if SN
            fxController.StartTeleport();
#else
            fxController.StartTeleport(null);
#endif
            yield return new WaitForSeconds(delay);
            fxController.StopTeleport();
        }

        [HarmonyPatch(nameof(Vehicle.OnUpgradeModuleUse))]
        public static void Postfix(Vehicle __instance, TechType techType)
        {
            if (techType == MiniatureVehicleModule.thisTechType)
            {
                if (__instance.transform.localScale == new Vector3(1, 1, 1))
                {
                    __instance.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                    __instance.transform.position += 0.5f * __instance.transform.up;
                }
                else
                    __instance.transform.localScale = new Vector3(1, 1, 1);
            }
        }
    }
}
