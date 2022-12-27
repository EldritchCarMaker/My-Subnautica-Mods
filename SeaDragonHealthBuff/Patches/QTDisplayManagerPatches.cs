using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FCS_AlterraHub.Enumerators;
using FCS_AlterraHub.Mono;
using FCS_AlterraHub.Registration;
using FCS_HomeSolutions.Mods.QuantumTeleporter.Buildable;
using FCS_HomeSolutions.Mods.QuantumTeleporter.Enumerators;
using FCS_HomeSolutions.Mods.QuantumTeleporter.Interface;
using FCS_HomeSolutions.Mods.QuantumTeleporter.Mono;
using FCSCommon.Utilities;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using static FCS_HomeSolutions.Mods.QuantumTeleporter.Mono.QTDisplayManager;
using Logger = QModManager.Utility.Logger;

namespace AdaptiveTeleportingCosts.Patches
{
    [HarmonyPatch(typeof(QTDisplayManager))]
    internal class QTDisplayManagerPatches
    {
        [HarmonyPatch(nameof(QTDisplayManager.OnButtonClick))]
        public static bool Prefix(QTDisplayManager __instance, string btnName, object tag)
        {
            switch (btnName)
            {
                case "RenameBTN":
                    __instance._mono.NameController.Show();
                    break;
                case "HomeNetworkBTN":
                    __instance.SetTab(QTTeleportTypes.Intra);
                    break;
                case "GlobalNetworkBTN":
                    __instance.SetTab(QTTeleportTypes.Global);
                    break;
                case "ToggleGlobalNetworkBTN":
                    __instance._mono.ToggleIsGlobal();
                    break;
                case "NetworkItem":
                    __instance._toUnit = (QuantumTeleporterController)tag;

                    var cost = TeleportUtils.GetTeleportCost(__instance._mono, __instance._toUnit);
                    if (__instance._toUnit.PowerManager.PowerAvailable() >= cost && __instance._mono.PowerManager.PowerAvailable() >= cost)
                    {
                        __instance._destination.text = $"[{__instance._toUnit.GetName()}]";
                        __instance.GotoPage(QttPages.Confirmation);
                    }

                    break;
                case "CancelBTN":
                    __instance.GotoPage(QttPages.Destinations);
                    break;
                case "ConfirmBTN":
                    __instance.GotoPage(QttPages.Destinations);
                    TeleportManager.TeleportPlayer(__instance._mono, __instance._toUnit, __instance.SelectedTab);
                    break;
            }
            return false;
        }
        [HarmonyPatch(nameof(QTDisplayManager.OnLoadDisplay))]
        public static bool Prefix(QTDisplayManager __instance, DisplayData data)
        {
            List<FcsDevice> items = null;

            __instance._teleportGrid.ClearPage();

            if (__instance._mono.Manager != null)
            {
                if (__instance.SelectedTab == QTTeleportTypes.Global)
                {
                    items = new List<FcsDevice>();
                    foreach (var device in FCSAlterraHubService.PublicAPI.GetRegisteredDevicesOfId(QuantumTeleporterBuildable.QuantumTeleporterTabID))
                    {
                        var fcsDevice = (QuantumTeleporterController)device.Value;
                        if (fcsDevice.IsGlobal && fcsDevice.Manager != __instance._mono.Manager)
                        {
                            items.Add(device.Value);
                        }
                    }
                }
                else if (__instance.SelectedTab == QTTeleportTypes.Intra)
                {
                    items = __instance._mono.Manager.GetDevices(QuantumTeleporterBuildable.QuantumTeleporterTabID).Where(x => x.Manager == __instance._mono.Manager)
                        .ToList();
                }

                if (items == null)
                {
                    Logger.Log(Logger.Level.Error, "Items list returned null");
                    return false;
                }

                if (data.EndPosition > items.Count)
                {
                    data.EndPosition = items.Count;
                }

                for (int i = data.StartPosition; i < data.EndPosition; i++)
                {
                    var unit = items[i];
                    var unitName = ((QuantumTeleporterController)unit).GetName();

                    if (unit == __instance._mono) continue;

                    GameObject itemDisplay = GameObject.Instantiate(data.ItemsPrefab);

                    if (itemDisplay == null || data.ItemsGrid == null)
                    {
                        if (itemDisplay != null)
                        {
                            GameObject.Destroy(itemDisplay);
                        }
                        return false;
                    }

                    itemDisplay.transform.SetParent(data.ItemsGrid.transform, false);
                    var textField = itemDisplay.transform.Find("Name").gameObject;
                    var text = textField.GetComponent<Text>();
                    text.text = unitName;

                    var itemButton = itemDisplay.AddComponent<InterfaceButton>();
                    var status = itemDisplay.AddComponent<AdaptiveStatusUpdater>();
                    status.Unit = unit;
                    itemButton.ButtonMode = InterfaceButtonMode.TextColor;
                    itemButton.Tag = unit;
                    itemButton.TextComponent = text;
                    itemButton.GetAdditionalDataFromString = true;
                    var powerDisplayHolder = itemDisplay.AddComponent<PowerDisplayHolder>();
                    itemButton.GetAdditionalString += powerDisplayHolder.GetPower;
                    itemButton.OnButtonClick += __instance.OnButtonClick;
                    itemButton.BtnName = "NetworkItem";
                    itemButton.MaxInteractionRange = 5;

                    Logger.Log(Logger.Level.Debug, $"Added Unit {unitName}");
                }
            }

            __instance._teleportGrid.UpdaterPaginator(items?.Count ?? 0);
            return false;
        }
        public class PowerDisplayHolder : MonoBehaviour
        {
            public IQuantumTeleporter teleporter;
            public void Awake()
            {
                teleporter = GetComponentInParent<IQuantumTeleporter>();
            }
            public string GetPower(object device)
            {
                var destinationTele = (QuantumTeleporterController)device;

                return $"Power: {destinationTele.Manager.Habitat.powerRelay.GetPower()}/{destinationTele.Manager.Habitat.powerRelay.GetMaxPower()}, Cost: {TeleportUtils.GetTeleportCost(destinationTele, teleporter)}";

                return $"{Language.main.GetFormat("HUDPowerStatus", destinationTele.Manager.Habitat.powerRelay.GetPower(), (TeleportUtils.GetTeleportCost(destinationTele, teleporter)))}";
            } 
        }
    }
}
