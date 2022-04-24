using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using System;
using Logger = QModManager.Utility.Logger;
using UnityEngine;
using System.Collections;
using UWE;
using SMLHelper.V2.Utility;
using System.IO;
using Sprite = Atlas.Sprite;
using static RemoteControlVehicles.TeleportVehicleModule;
using UnityEngine.UI;

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
        public static Text text1;
        public static Text text2;

        [HarmonyPatch(typeof(Vehicle), nameof(Vehicle.OnPilotModeEnd))]
        [HarmonyPostfix]
        public static void TeleportPlayer(Vehicle __instance)
        {
            if(isActive)
            {
                Player.main.currentSub = sub;
                Player.main.transform.position = position;
                isActive = false;
                hud.SetActive(false);
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
            text1 = textTransform.Find("TitleText").gameObject.GetComponent<Text>();
            text2 = textTransform.Find("DistanceText").gameObject.GetComponent<Text>();
            hud.SetActive(false);
        }

        [HarmonyPatch(typeof(Player), nameof(Player.Update))]
        [HarmonyPostfix]
        public static void StartControl(Player __instance)
        {
            if (Input.GetKeyUp(QMod.config.ControlKey))
            {
                if (!PlayerHasChip()) return;

                if (vehicle == null)
                {
                    ErrorMessage.AddMessage("No vehicle available to control");
                    return;
                }

                if (isActive)
                {
                    ErrorMessage.AddMessage("Already controlling vehicle");
                    return;
                }

                if (QMod.config.MustBeInBase && !__instance.IsInSub())
                {
                    ErrorMessage.AddMessage("Must be in safe location to control vehicle");
                    return;
                }

                sub = Player.main.currentSub;
                position = Player.main.transform.position;
                text1.text = vehicle.subName.GetName();

                hud.SetActive(true);
                hud.transform.Find("Connecting").gameObject.SetActive(true);
                blackScreen.SetActive(true);
                Image image = blackScreen.GetComponent<Image>();
                Color color = image.color;
                color.a = 1;
                image.color = color;

                CoroutineHost.StartCoroutine(WaitForWorld(__instance));
            }
        }
        public static bool PlayerHasChip()
        {
            List<string> chipSlots = new List<string>(); 
            Inventory.main.equipment.GetSlots(EquipmentType.Chip, chipSlots);
            Equipment e = Inventory.main.equipment;

            foreach (string slot in chipSlots)
            {
                TechType tt = e.GetTechTypeInSlot(slot);
                if (tt == RemoteControlHudChip.thisTechType)
                {
                    return true;
                }
            }
            return false;
        }
        public static IEnumerator WaitForWorld(Player player)
        {
            player.transform.position = vehicle.transform.position;
            yield return new WaitUntil(() => LargeWorldStreamer.main.IsWorldSettled());

            hud.transform.Find("Connecting").gameObject.SetActive(false);

            Image image = blackScreen.GetComponent<Image>();
            Color color = image.color;
            color.a = 0;
            image.color = color;
            if (vehicle.docked)
            {
                vehicle.GetComponentInParent<SubRoot>().GetComponentInChildren<DockedVehicleHandTarget>().OnHandClick(Player.main.armsController.guiHand);
                /*
                VehicleDockingBay dockingbay = vehicle.GetComponentInParent<VehicleDockingBay>();
                if(dockingbay == null)
                {
                    SubRoot subRoot = vehicle.GetComponentInParent<SubRoot>();
                    dockingbay = subRoot.GetComponentInChildren<VehicleDockingBay>();
                }
                Vehicle dockedVehicle = dockingbay.GetDockedVehicle();
                dockingbay.vehicle_docked_param = false;
                CoroutineHost.StartCoroutine(WaitForABit(dockingbay, __instance, dockedVehicle));
                */
            }
            else
            {
                vehicle.EnterVehicle(player, true, false);
            }
            isActive = true;
        }
        public static IEnumerator WaitForABit(VehicleDockingBay dockingBay, Player player, Vehicle dockedVehicle)//unused
        {
            yield return new WaitForSeconds(2f);
            dockingBay.OnUndockingComplete(player);
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
                Player.main.transform.position = position;
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