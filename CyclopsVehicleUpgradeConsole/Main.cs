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
using UnityEngine.EventSystems;

namespace CyclopsVehicleUpgradeConsole
{
    [HarmonyPatch]
    public static class MakeThing
    {

        //Todo -

        //Understand code

        //Make buttons to create vehicles from within moonpool



        [HarmonyPatch(typeof(SubRoot), nameof(SubRoot.Start))]
        [HarmonyPostfix]
        public static void Something(SubRoot __instance)
        {
            if (!__instance.isCyclops) return;

            VehicleDockingBay dockingBay = __instance.GetComponentInChildren<VehicleDockingBay>();
            CyclopsVehicleStorageTerminalManager cyclopsConsole = __instance.GetComponentInChildren<CyclopsVehicleStorageTerminalManager>();
            if (cyclopsConsole == null) return;

            //check if cyclops already has the console
            if (cyclopsConsole.GetComponentInChildren<SubNameInput>(true) != null) return;

            CoroutineHost.StartCoroutine(MakeUpgradeConsole(cyclopsConsole, dockingBay));
        }
        [HarmonyPatch(typeof(VehicleDockingBay), nameof(VehicleDockingBay.DockVehicle))]
        [HarmonyPostfix]
        public static void SetTarget(VehicleDockingBay __instance, Vehicle vehicle)
        {
            SubRoot subRoot = __instance.GetComponentInParent<SubRoot>();

            if (!subRoot.isCyclops) return;

            SubNameInput subNameInput = subRoot.GetComponentInChildren<CyclopsVehicleStorageTerminalManager>().GetComponentInChildren<SubNameInput>(true);
            subNameInput.SetTarget(vehicle.subName);
            SetActive(__instance.gameObject);
        }
        [HarmonyPatch(typeof(VehicleDockingBay), nameof(VehicleDockingBay.OnUndockingStart))]
        [HarmonyPostfix]
        public static void RemoveTarget(VehicleDockingBay __instance)
        {
            SubRoot subRoot = __instance.GetComponentInParent<SubRoot>();

            if (!subRoot.isCyclops) return;

            SubNameInput subNameInput = subRoot.GetComponentInChildren<CyclopsVehicleStorageTerminalManager>().GetComponentInChildren<SubNameInput>(true);
            subNameInput.SetTarget(null);
            SetInActive(__instance.gameObject);
        }





        public static void SetActive(GameObject gameObject)
        {
            GameObject vehicleTerminal = gameObject.GetComponentInParent<SubRoot>().transform.Find("CyclopsVehicleStorageTerminal").gameObject;

            if (vehicleTerminal == null) return;

            GameObject screen = vehicleTerminal.transform.Find("GUIScreen").gameObject;
            for(var i = 0; i < screen.transform.childCount; i++)
            {
                GameObject child = screen.transform.GetChild(i).gameObject;
                if(!child.name.Equals("Swap Button"))
                {
                    child.SetActive(false);
                }
                else
                {
                    child.GetComponent<SwapButton>().colorScreenActive = false;
                }
            }
            vehicleTerminal.GetComponent<CyclopsVehicleStorageTerminalManager>().OnDockedChanged();

            vehicleTerminal.transform.Find("EditScreen").gameObject.GetComponent<SubNameInput>().uiActive.SetActive(false);
        }
        public static void SetInActive(GameObject gameObject)
        {
            GameObject vehicleTerminal = gameObject.GetComponentInParent<SubRoot>().transform.Find("CyclopsVehicleStorageTerminal").gameObject;

            if (vehicleTerminal == null) return;

            GameObject screen = vehicleTerminal.transform.Find("GUIScreen").gameObject;
            for (var i = 0; i < screen.transform.childCount; i++)
            {
                GameObject child = screen.transform.GetChild(i).gameObject;
                if (!child.name.Equals("Swap Button"))
                {
                    child.SetActive(false);
                }
                else
                {
                    child.GetComponent<SwapButton>().colorScreenActive = true;
                }
            }
            vehicleTerminal.transform.Find("EditScreen").gameObject.GetComponent<SubNameInput>().uiActive.SetActive(true);
        }




        public static IEnumerator MakeUpgradeConsole(CyclopsVehicleStorageTerminalManager cyclopsConsole, VehicleDockingBay dockingBay)
        {
            yield return new WaitUntil(() => Base.pieces != null && !Base.pieces[(int)Base.Piece.MoonpoolUpgradeConsole].Equals(default(Base.PieceDef)) && Base.pieces[(int)Base.Piece.MoonpoolUpgradeConsole].prefab != null);

            var prefab = Base.pieces[(int)Base.Piece.MoonpoolUpgradeConsole].prefab.gameObject;
            SubNameInput moonpoolConsole = prefab.GetComponentInChildren<SubNameInput>();
            
            GameObject cyclopsConsoleGUI = cyclopsConsole.gameObject.transform.Find("GUIScreen").gameObject;

            GameObject gameObject4 = GameObject.Instantiate(moonpoolConsole.gameObject);
            gameObject4.transform.position = cyclopsConsoleGUI.transform.position;
            gameObject4.gameObject.transform.rotation = cyclopsConsoleGUI.gameObject.transform.rotation;
            gameObject4.transform.SetParent(cyclopsConsole.gameObject.transform);
            gameObject4.name = "EditScreen";
            GameObject.Destroy(gameObject4.transform.Find("Inactive").gameObject);
            Vehicle dockedVehicle = dockingBay.GetDockedVehicle();

            gameObject4.transform.Find("Active").gameObject.SetActive(dockedVehicle != null);
            if (dockedVehicle != null)
            {
                gameObject4.GetComponent<SubNameInput>().SetTarget(dockedVehicle.subName);
            }

            /*
            GameObject buttonObject = new GameObject("Button Object");
            buttonObject.EnsureComponent<SwapButton>();
            buttonObject.transform.position = cyclopsConsoleGUI.transform.position + new Vector3(0, 0, 1);
            buttonObject.SetActive(true);
            */
            GameObject button = GameObject.Instantiate(cyclopsConsoleGUI.gameObject.transform.Find("Seamoth").Find("Modules").gameObject.GetComponent<CyclopsVehicleStorageTerminalButton>().gameObject, cyclopsConsoleGUI.gameObject.transform);

            button.transform.position += 0.5f * button.gameObject.transform.right;
            button.transform.position += 0.15f * button.gameObject.transform.up;

            button.AddComponent<SwapButton>();

            GameObject.Destroy(button.GetComponent<CyclopsVehicleStorageTerminalButton>());
            button.name = "Swap Button";
        }
    }
    public class SwapButton : HandTarget, IHandTarget, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerClickHandler
    {
        private string hoverText = "Swap Screens";
        public bool colorScreenActive = false;
        public void OnHandClick(GUIHand hand)
        {
            if(colorScreenActive)
            {
                colorScreenActive = false;
                MakeThing.SetActive(gameObject);
            }
            else
            {
                colorScreenActive = true;
                MakeThing.SetInActive(gameObject);
            }
        }

        public void OnHandHover(GUIHand hand)
        {
            if (hand.IsFreeToInteract())
            {
                HandReticle.main.SetIcon(HandReticle.IconType.Hand, 1f);
                HandReticle.main.SetInteractText(this.hoverText);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnHandClick(null);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            HandReticle.main.SetIcon(HandReticle.IconType.Hand, 1f);
            HandReticle.main.SetInteractText(this.hoverText);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            HandReticle.main.SetIcon(HandReticle.IconType.Default, 1f);
            HandReticle.main.SetInteractText("");
        }
    }
}

/*
 * var cyclopsVehicleStorageTerminalButton = geti<CyclopsVehicleStorageTerminalButton>()
> GameObject button = GameObject.Instantiate(cyclopsVehicleStorageTerminalButton.gameObject, cyclopsVehicleStorageTerminalButton.gameObject.transform.parent);

> button.transform.position += 0.5f * button.gameObject.transform.right;
> button.transform.position += 0.15f * button.gameObject.transform.up;

button.AddComponent<CyclopsVehicleUpgradeConsole.SwapButton>();

GameObject.Destroy(button.GetComponent<CyclopsVehicleStorageTerminalButton>());


public class CyclopsVehicleStorageTerminalButton : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerClickHandler
*/