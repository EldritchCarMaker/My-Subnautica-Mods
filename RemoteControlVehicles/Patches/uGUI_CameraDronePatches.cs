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
            RemoteControlVehicle vehicle = RemoteControlVehicle.currentVehicle;

            if (!vehicle || !vehicle.isActiveAndEnabled || !vehicle.controllingPlayer || __instance.activeCamera)
                return true;

            __instance.content.SetActive(true);
            /*
            float a2 = __instance.faderSequence.target ? 1f : 0f;
            if (__instance.faderSequence.active)
            {
                __instance.faderSequence.Update();
                a2 = 0.5f * (1f - Mathf.Cos(3.1415927f * __instance.faderSequence.t));//what
            }
            

            Color color = __instance.fader.color;
            color.a = a2;
            __instance.fader.color = color;
            */
            __instance.fader.color = new Color(0, 0, 0, 0);

            if (vehicle.CanBeControlled())
            {
                if (__instance.waitForCamera)
                {
                    __instance.connecting.SetActive(true);
                    __instance.waitForCamera = true;
                    if (vehicle.IsReady())
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

                    __instance.textTitle.text = vehicle.VehicleName;
                    __instance.UpdateDistanceText((int)vehicle.GetDistance());
                    int num = -1;
                    LiveMixin liveMixin = vehicle.liveMixin;
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
                    EnergyMixin energyMixin = vehicle.energyMixin;
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
