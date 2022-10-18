using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using FCS_AlterraHub.Mods.FCSPDA.Mono.Dialogs;
using FCS_AlterraHub.Buildables;
using FCS_AlterraHub.Mods.AlterraHubFabricatorBuilding.Mono.DroneSystem;
using FCS_AlterraHub.Mono;
using FCS_AlterraHub.Registration;
using UnityEngine;
using FCS_AlterraHub.Configuration;
using FCS_AlterraHub.Mods.AlterraHubDepot.Mono;
using NoFCSDronePort.Monobehaviours;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine.UI;
using FCSCommon.Utilities;

namespace NoFCSDronePort.Patches
{
    [HarmonyPatch(typeof(DestinationDialogController))]
    internal class DestinationDialogControllerPatches
    {
        [HarmonyPatch(nameof(DestinationDialogController.RefreshAlterraHubDepotList))]
        public static bool Prefix(DestinationDialogController __instance)
        {
            for (int i = __instance._toggles.Count - 1; i >= 0; i--)
            {
                AlterraHubDepotItemController alterraHubDepotItemController = __instance._toggles[i];
                alterraHubDepotItemController.UnRegisterAndDestroy();
                __instance._toggles.Remove(alterraHubDepotItemController);
            }
            foreach (KeyValuePair<string, FcsDevice> keyValuePair in FCSAlterraHubService.PublicAPI.GetRegisteredDevicesOfId(Mod.AlterraHubDepotTabID))
            {
                AlterraHubDepotController alterraDronePortController = keyValuePair.Value as AlterraHubDepotController;
                if (!alterraDronePortController)
                {
                    QuickLogger.Error($"Value is not depot controller! key: {keyValuePair.Key}, value: {keyValuePair.Value}");
                    continue;
                }

                if (alterraDronePortController.IsOperational)
                {
                    GameObject depotPrefab = UnityEngine.Object.Instantiate<GameObject>(AlterraHub.AlterraHubDepotItemPrefab);
                    AlterraHubDepotItemController controller = depotPrefab.AddComponent<AlterraHubDepotItemController>();

                    AlterraHubDepotItemControllerNoDrone controllerNoDrone = depotPrefab.AddComponent<AlterraHubDepotItemControllerNoDrone>();

                    if (controllerNoDrone.Initialize(controller, alterraDronePortController, __instance._toggleGroup, __instance._list))
                    {
                        __instance._toggles.Add(controller);
                    }
                    else
                    {
                        UnityEngine.Object.Destroy(depotPrefab);
                    }
                }
            }

            return false;
        }
        [HarmonyPatch(nameof(DestinationDialogController.Initialize))]
        public static bool Prefix(CheckOutPopupDialogWindow checkoutWindow, DestinationDialogController __instance)
        {
            __instance._checkoutWindow = checkoutWindow;
            __instance.gameObject.FindChild("Content").FindChild("GameObject").FindChild("CancelBTN").GetComponent<Button>().onClick.AddListener(delegate ()
            {
                __instance.Close(true);
            });
            __instance.gameObject.FindChild("Content").FindChild("GameObject").FindChild("DoneBTN").GetComponent<Button>().onClick.AddListener(delegate ()
            {
                foreach (AlterraHubDepotItemController controller in __instance._toggles)
                {
                    if (controller.IsChecked)
                    {
                        var controllerNoDrone = controller.GetComponent<AlterraHubDepotItemControllerNoDrone>();

                        var checkOutWindowNoDrone = __instance._checkoutWindow.gameObject.EnsureComponent<CheckOutPopupDialogWindowNoDrone>();

                        checkOutWindowNoDrone.Normal = __instance._checkoutWindow;

                        checkOutWindowNoDrone.SelectedDestination = controllerNoDrone.Destination;
                        break;
                    }
                }
                __instance.Close(false);
            });
            __instance._toggleGroup = __instance.gameObject.GetComponentInChildren<ToggleGroup>();
            __instance._list = __instance._toggleGroup.gameObject.transform;

            return false;
        }
    }
}
