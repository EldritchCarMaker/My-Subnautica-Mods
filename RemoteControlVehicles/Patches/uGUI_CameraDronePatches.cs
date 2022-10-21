using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RemoteControlVehicles.Monobehaviours;
using UnityEngine;

namespace RemoteControlVehicles.Patches
{
    [HarmonyPatch(typeof(uGUI_CameraDrone))]
    internal class uGUI_CameraDronePatches
    {
        [HarmonyPatch(nameof(uGUI_CameraDrone.LateUpdate))]
        public static bool Prefix(uGUI_CameraDrone __instance)
        {
            var aurora = RemoteControlAuroraMono.lastUsedMono;
            if (!aurora || !aurora.controllingPlayer || __instance.activeCamera)
                return true;

            __instance.content.SetActive(true);
            float a = 0f;
            float a2 = __instance.faderSequence.target ? 1f : 0f;
            if (__instance.faderSequence.active)
            {
                __instance.faderSequence.Update();
                a = (a2 = 0.5f * (1f - Mathf.Cos(3.1415927f * __instance.faderSequence.t)));
            }

            /*
             * Handles fading and shit based off of distance
             * Don't want. RC Aurora has infinite range.
            float b = (__instance.activeCamera != null) ? (Mathf.Max(0f, __instance.activeCamera.GetScreenDistance(null) - 250f) / 250f) : 0f;
            __instance._noise = Mathf.Max(a, b);
            float b2 = (__instance.activeCamera != null) ? Mathf.Clamp((__instance.activeCamera.GetScreenDistance(null) - 520f) / 100f, 0f, 0.99f) : 0f;
            Color color = __instance.fader.color;
            color.a = Mathf.Max(a2, b2);
            __instance.fader.color = color;
            */


            if (aurora.CanBeControlled())
            {
                if (__instance.waitForCamera)
                {
                    __instance.connecting.SetActive(true);
                    __instance.waitForCamera = true;
                    if (aurora.IsReady())
                    {
                        __instance.connecting.SetActive(false);
                        __instance.waitForCamera = false;
                        __instance.faderSequence.Set(1f, false, null);
                    }
                }
                else
                {
                    __instance.noSignal.SetActive(false);
                    __instance.connecting.SetActive(false);

                    __instance.textTitle.text = "Aurora";
                    __instance.UpdateDistanceText((int)aurora.GetDistance());
                    int num = -1;
                    LiveMixin liveMixin = aurora.liveMixin;
                    if (liveMixin != null)
                    {
                        num = Mathf.RoundToInt(100f * (liveMixin.health / liveMixin.maxHealth));
                    }
                    if (__instance.health != num)
                    {
                        __instance.health = num;
                        __instance.textHealth.text = IntStringCache.GetStringForInt(__instance.health);
                    }
                    int num2 = -1;
                    EnergyMixin energyMixin = aurora.energyMixin;
                    if (energyMixin != null)
                    {
                        num2 = Mathf.RoundToInt(100f * (energyMixin.charge / energyMixin.capacity));
                    }
                    if (__instance.power != num2)
                    {
                        __instance.power = num2;
                        __instance.textPower.text = IntStringCache.GetStringForInt(__instance.power);
                    }
                    __instance.UpdatePing();
                }
            }
            else
            {
                __instance.UpdateDistanceText(-1);
                __instance.faderSequence.ForceState(true);
                __instance.noSignal.SetActive(true);
                __instance.connecting.SetActive(false);
            }
            HandReticle.main.SetUseTextRaw(__instance.stringControls, string.Empty);
            return false;
        }
    }
}
