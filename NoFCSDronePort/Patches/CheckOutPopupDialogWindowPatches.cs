using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using FCS_AlterraHub.Mods.FCSPDA.Mono.Dialogs;
using NoFCSDronePort.Monobehaviours;
using FCS_AlterraHub.Mods.FCSPDA.Mono;
using FCS_AlterraHub.Helpers;
using UnityEngine;
using FCS_AlterraHub.Buildables;
using FCS_AlterraHub.Configuration;
using FCS_AlterraHub.Model;
using FCS_AlterraHub.Mods.FCSPDA.Mono.ScreenItems;
using FCS_AlterraHub.Systems;
using FCS_AlterraHub.Mods.AlterraHubFabricatorBuilding.Mono.DroneSystem;
using FCS_AlterraHub.Mods.AlterraHubFabricatorBuilding.Mono;
using FCSCommon.Utilities;
using FCS_AlterraHub.Mods.AlterraHubDepot.Mono;
using FCS_AlterraHub.Mono;
using System.Collections;
using UWE;

namespace NoFCSDronePort.Patches
{
    [HarmonyPatch(typeof(CheckOutPopupDialogWindow))]
    internal class CheckOutPopupDialogWindowPatches
    {
        [HarmonyPatch(nameof(CheckOutPopupDialogWindow.CreateDestinationPopup))]
        public static bool Prefix()
        {
            var __instance = FCSPDAController.Main._checkoutDialog;
            GameObject gameObject = GameObjectHelpers.FindGameObject(__instance._mono.ui.gameObject, "DestinationPopUp", SearchOption.Full);
            __instance._destinationDialogController = gameObject.AddComponent<DestinationDialogController>();
            __instance._destinationDialogController.Initialize(__instance);
            DestinationDialogController destinationDialogController = __instance._destinationDialogController;
            __instance._destinationDialogController.OnClose += () => 
            {
                var destination = __instance.GetComponent<CheckOutPopupDialogWindowNoDrone>()?.SelectedDestination;
                __instance._destinationText.text = $"Destination: {destination?.Manager.GetBaseName()}";
            };
            return false;
        }
        [HarmonyPatch(nameof(CheckOutPopupDialogWindow.MakePurchase))]
        public static bool Prefix(CheckOutPopupDialogWindow __instance, ref bool __result)
        {
            List<Vector2int> list = new List<Vector2int>();
            foreach (CartItem cartItem in __instance._cart.GetItems())
            {
                for (int i = 0; i < cartItem.ReturnAmount; i++)
                {
                    list.Add(CraftData.GetItemSize(cartItem.TechType));
                }
            }

            if (!CardSystem.main.HasBeenRegistered())
            {
                MessageBoxHandler.main.Show(AlterraHub.AccountNotFoundFormat(), FCSMessageButton.OK, null);
                __result =  false;
                return false;
            }
            if (Inventory.main.container.GetCount(Mod.DebitCardTechType) <= 0)
            {
                MessageBoxHandler.main.Show(AlterraHub.CardNotDetected(), FCSMessageButton.OK, null);
                __result =  false;
                return false;
            }
            if (!CardSystem.main.HasEnough(__instance._cart.GetTotal()))
            {
                MessageBoxHandler.main.Show(AlterraHub.NotEnoughMoneyOnAccount(), FCSMessageButton.OK, null);
                __result =  false;
                return false;
            }

            if (!QMod.config.needDepo)
            {
                MakeAPurchase(__instance._cart, null, true);
                return false;
            }
            var noDrone = __instance.GetComponent<CheckOutPopupDialogWindowNoDrone>();
            if (noDrone.SelectedDestination == null)
            {
                MessageBoxHandler.main.Show(AlterraHub.NoDestinationFound(), FCSMessageButton.OK, null);
                __result = false;
                return false;
            }

            if (!MakeAPurchase(__instance._cart, noDrone.SelectedDestination, false))
            {
                __result = false;
                return false;
            }
            __instance._cart.TransactionComplete();
            __instance.HideDialog();
            __result = true;
            return false;
        }

        public static bool HasRoomFor(AlterraHubDepotController depot, List<Vector2int> sizes)
        {
            return depot.GetFreeSlotsCount() >= sizes.Count;

            //don't want to do this, could lead to issues later down the line. Just gotta deal with the inventory size of one for now
            IEnumerable<FcsDevice> devices = depot.Manager.GetDevices("AHD");
            int num = 0;
            foreach (FcsDevice fcsDevice in devices)
            {
                if (!(fcsDevice == null))
                {
                    AlterraHubDepotController alterraHubDepotController = fcsDevice as AlterraHubDepotController;
                    if (alterraHubDepotController != null)
                    {
                        num += alterraHubDepotController.GetFreeSlotsCount();
                    }
                }
            }
            return num >= sizes.Count;
        }
        public static bool MakeAPurchase(CartDropDownHandler cart, AlterraHubDepotController depot = null, bool giveToPlayer = false)
        {
            var totalCash = cart.GetTotal();
            var sizes = FCSPDAController.GetSizes(cart);
            if (giveToPlayer)
            {
                if (CardSystem.main.HasEnough(totalCash) && Inventory.main.container.HasRoomFor(sizes))
                {
                    foreach (CartItem item in cart.GetItems())
                    {
                        for (int i = 0; i < item.ReturnAmount; i++)
                        {
                            QuickLogger.Debug($"{item.ReceiveTechType}", true);
                            PlayerInteractionHelper.GivePlayerItem(item.ReceiveTechType);
                        }
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (depot == null || !HasRoomFor(depot, sizes))
                {
                    MessageBoxHandler.main.Show(depot == null ? AlterraHub.DepotNotFound() : AlterraHub.DepotFull(), FCSMessageButton.OK);
                    return false;
                }

                if (AlterraFabricatorStationController.Main == null)
                {
                    QuickLogger.Error("FCSStation Main is null!");
                    QuickLogger.ModMessage("The FCSStation cannot be found please contact FCSStudios for help with this issue. Order will be sent to your inventory");
                    MakeAPurchase(cart, null, true);
                    return true;
                }

                Purchase(depot, cart);
            }

            CardSystem.main.RemoveFinances(totalCash);
            return true;
        }
        public static void Purchase(AlterraHubDepotController depot, CartDropDownHandler cart)
        {
            var cartItemSaveData = cart.Save().ToList<CartItemSaveData>();

            string text = Guid.NewGuid().ToString("n").Substring(0, 8);
            Shipment shipment = new Shipment
            {
                CartItems = cartItemSaveData,
                OrderNumber = text,
                Port = null,
                PortPrefabID = depot.GetPrefabID()
            };
            /*
            if (!QMod.config.mimicDroneTravel)
                AlterraDronePortController.OffloadItemsInBase(shipment, new List<FcsDevice>() { depot });
            else
                CoroutineHost.StartCoroutine((IEnumerator)OffloadItemsAfterDelay(
                    new Action(() => 
                    { 
                        AlterraDronePortController.OffloadItemsInBase(shipment, new List<FcsDevice>() { depot });
                        if(QMod.config.sendMessage) ErrorMessage.AddMessage($"Order delivered to base {depot.Manager.GetBaseName()}");
                    })));//that's a lot of parenthesis all together
            */
            AlterraDronePortController.OffloadItemsInBase(shipment, new List<FcsDevice>() { depot });

            VoiceNotificationSystem.main.DisplayMessage(AlterraHub.OrderHasBeenShipped() + " " + depot.Manager.GetBaseName(), null);
        }
        /*
        public static IEnumerable OffloadItemsAfterDelay(Action action)
        {
            var time = new System.Random().Next(QMod.config.minRandomTimeForMimic, QMod.config.maxRandomTimeForMimic);
            yield return new WaitForSeconds(time);
            action();
        }
        */
    }
}
