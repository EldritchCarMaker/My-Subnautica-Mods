﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using System;
using UnityEngine;
using System.Collections;
using UWE;
using System.IO;
using Sprite = Atlas.Sprite;
using static RemoteControlVehicles.TeleportVehicleModule;
using UnityEngine.UI;
using TMPro;

namespace RemoteControlVehicles
{
    [HarmonyPatch]
    public static class RemoteControlVehicles
    {
        public static SubRoot sub;
        public static Vehicle vehicle;
        public static Vector3 position;
        public static bool hasModule = false;
        public static bool isActive = false;
        public static GameObject hud;
        public static GameObject blackScreen;
#if SN1
        public static Text text1 { get; set; }
        public static Text text2 { get; set; }
#else
        public static TextMeshProUGUI text1 { get; set; }
        public static TextMeshProUGUI text2 { get; set; }
#endif

        public static Vector3 subPosition;

        [HarmonyPatch(typeof(Vehicle), nameof(Vehicle.OnPilotModeEnd))]
        [HarmonyPostfix]
        public static void TeleportPlayer(Vehicle __instance)
        {
            if(isActive)
            {
                Player.main.currentSub = sub;

                Vector3 difference = new Vector3(0, 0, 0);

                if(sub != null)
                {
                    var newSubPosition = sub.transform.position;
                    difference = subPosition - newSubPosition;
                }

                Player.main.SetPosition(position - difference);
                isActive = false;
                hud.SetActive(false);
                Player.main.liveMixin.shielded = false;
            }
        }
        [HarmonyPatch(typeof(Player), nameof(Player.ExitPilotingMode))]
        [HarmonyPostfix]
        public static void TeleportPlayer(Player __instance)
        {
            if (isActive)
            {
                /*
                __instance.currentSub = null;
                __instance.transform.position = position;
                __instance.currentSub = sub;*/
                isActive = false;
                hud.SetActive(false);
                __instance.liveMixin.shielded = false;
                CoroutineHost.StartCoroutine(WaitForABit(null, __instance, null));
            }
        }

        [HarmonyPatch(typeof(Player), nameof(Player.Start))]
        [HarmonyPostfix]
        public static void MakeHUD(Player __instance)
        {
            GameObject prefabHUD = uGUI_CameraDrone.main.connecting.transform.parent.gameObject;
            hud = GameObject.Instantiate(prefabHUD, prefabHUD.transform.parent);
            hud.name = "RemoteControlVehicleHud";

            //bad, but only is called once per game so should be good enough for now
            //do improve later though
            GameObject.Destroy(hud.transform.Find("PingCanvas").gameObject);
            GameObject.Destroy(hud.transform.Find("HealthBackground").gameObject);
            GameObject.Destroy(hud.transform.Find("PowerBackground").gameObject);
            GameObject.Destroy(hud.transform.Find("FrameLeft").gameObject);
            GameObject.Destroy(hud.transform.Find("FrameRight").gameObject);
            GameObject.Destroy(hud.transform.Find("Crosshair").gameObject);
            GameObject.Destroy(hud.transform.Find("Dot").gameObject);

            blackScreen = hud.transform.Find("Fader").gameObject;
            Transform textTransform = hud.transform.Find("Title");
#if SN1
            text1 = textTransform.Find("TitleText").gameObject.GetComponent<Text>();
            text2 = textTransform.Find("DistanceText").gameObject.GetComponent<Text>();
#else
            text1 = textTransform.Find("TitleText").gameObject.GetComponent<TextMeshProUGUI>();
            text2 = textTransform.Find("DistanceText").gameObject.GetComponent<TextMeshProUGUI>();
#endif

            hud.SetActive(false);
        }
        public static IEnumerator WaitForWorld(Player player, bool isCyclops = false)
        {
            if (!isCyclops)
            {
                if (!vehicle.docked)
                    player.SetPosition(vehicle.transform.position);

                yield return new WaitUntil(() => LargeWorldStreamer.main.IsWorldSettled());

                hud.transform.Find("Connecting").gameObject.SetActive(false);

                Image image = blackScreen.GetComponent<Image>();
                Color color = image.color;
                color.a = 0;
                image.color = color;
                if (vehicle.docked)
                {
                    vehicle.GetComponentInParent<SubRoot>().GetComponentInChildren<DockedVehicleHandTarget>().OnHandClick(Player.main.armsController.guiHand);
                }
                else
                {
                    vehicle.EnterVehicle(player, true, false);
                }
                isActive = true;
                player.liveMixin.shielded = true;
            }
            else
            {
                PilotingChair chair = CyclopsRemoteControlHandler.TrackedSub.GetComponentInChildren<PilotingChair>();

                Player.main.currentSub = null;

                player.SetPosition(chair.transform.position);
                yield return new WaitUntil(() => LargeWorldStreamer.main.IsWorldSettled());

                hud.transform.Find("Connecting").gameObject.SetActive(false);

                Image image = blackScreen.GetComponent<Image>();
                Color color = image.color;
                color.a = 0;
                image.color = color;

                Player.main.currentSub = CyclopsRemoteControlHandler.TrackedSub;
                chair.OnHandClick(player.armsController.guiHand);

                isActive = true;
                player.liveMixin.shielded = true;
            }
        }

        public static IEnumerator WaitForABit(VehicleDockingBay dockingBay, Player player, Vehicle dockedVehicle)
        {
            yield return new WaitForSeconds(1.1f);
            player.currentSub = null;


            Vector3 difference = new Vector3(0, 0, 0);

            if (sub != null)
            {
                var newSubPosition = sub.transform.position;
                difference = subPosition - newSubPosition;
            }

            player.SetPosition(position - difference);
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            player.currentSub = sub;
            yield return new WaitForEndOfFrame();
            if(Player.main.transform.position != position - difference)
            {
#if SN1
                Logger.Log(Logger.Level.Warn, "Player position not set, retrying");
#else
                QMod.Logger.LogInfo("Player position not set, retrying");
#endif

                for (var i=0; i<5; i++)
                {
                    yield return new WaitForEndOfFrame();
                }
                player.SetPosition(position - difference);            }
        }

        [HarmonyPatch(typeof(VehicleDockingBay), nameof(VehicleDockingBay.DockVehicle))]
        [HarmonyPrefix]
        public static void FixDocking(VehicleDockingBay __instance, Vehicle vehicle)
        {
            if(isActive)
            {
                vehicle.OnPilotModeEnd();
                if (!Player.main.ToNormalMode(true))
                {
                    Player.main.ToNormalMode(false);
                    Player.main.transform.parent = null;
                }
                Player.main.currentSub = sub;

                Vector3 difference = new Vector3(0, 0, 0);

                if (sub != null)
                {
                    var newSubPosition = sub.transform.position;
                    difference = subPosition - newSubPosition;
                }

                Player.main.SetPosition(position - difference);

                isActive = false;
                hud.SetActive(false);
            }
        }

        [HarmonyPatch(typeof(Vehicle), nameof(Vehicle.Update))]
        [HarmonyPostfix]
        public static void UpdatePatch(Vehicle __instance)
        {
            if (__instance.modules.GetCount(TeleportVehicleModule.thisTechType) > 0)
            {
                hasModule = true;
                vehicle = __instance;

                int distance = (int)(__instance.transform.position - position).magnitude;
                text2.text = "Distance: " + distance.ToString();

                text1.text = vehicle.subName.GetName();
                //text1.color = vehicle.subName.GetColor()
            }
        }
        [HarmonyPatch(typeof(Vehicle), nameof(Vehicle.OnUpgradeModuleChange))]
        [HarmonyPostfix]
        public static void RemoveModule(Vehicle __instance, TechType techType, bool added)
        {
            if(techType == TeleportVehicleModule.thisTechType)
            {
                hasModule = added;
                vehicle = null;
            }
        }

        [HarmonyPatch(typeof(Vehicle), nameof(Vehicle.ReplenishOxygen))]
        [HarmonyPrefix]
        public static bool FixOxygen(Vehicle __instance)
        {
            if (isActive && sub == null)
            {
                return false;
            }
            return true;
        }
        [HarmonyPatch(typeof(Player), nameof(Player.CanBreathe))]
        [HarmonyPostfix]
        public static void FixOxygen(Player __instance, ref bool __result)
        {
            if (isActive && sub == null)
            {
                __result = false;
            }
        }

        [HarmonyPatch(typeof(Vehicle), nameof(Vehicle.OnKill))]
        [HarmonyPrefix]
        public static void SetNoModuleOnKill(Vehicle __instance)
        {
            if(isActive)
            {
                hasModule = false;
            }else if(__instance.modules.GetCount(TeleportVehicleModule.thisTechType) > 0)
            {
                hasModule = false;
            }
        }
        [HarmonyPatch(typeof(Vehicle), nameof(Vehicle.OnKill))]
        [HarmonyPostfix]
        public static void DisconnectOnKill(Vehicle __instance)
        {
            if (isActive)
            {
                hasModule = false;
                CoroutineHost.StartCoroutine(LoseConnection());
            }
        }

        public static IEnumerator LoseConnection()
        {
            hud.transform.Find("NoConnection").gameObject.SetActive(true);

            ErrorMessage.AddMessage("Connection to vehicle lost");

            Image image = blackScreen.GetComponent<Image>();
            Color color = image.color;
            color.a = 1;
            image.color = color;

            yield return new WaitForSeconds(0.2f);

            color.a = 0;
            image.color = color;

            hud.SetActive(false);
        }

        [HarmonyPatch(typeof(Equipment), nameof(Equipment.AllowedToAdd))]
        [HarmonyPostfix]
        public static void EnsureOneModule(ref bool __result, Pickupable pickupable)
        {
            if (pickupable == null) return;
            if(pickupable.GetTechType() == TeleportVehicleModule.thisTechType && hasModule)
            {
                __result = false;
                ErrorMessage.AddMessage("Only one vehicle can have this module equipped at a time!");
            }
        }
    }
}