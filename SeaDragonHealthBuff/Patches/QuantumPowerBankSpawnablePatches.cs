using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FCS_AlterraHub.Extensions;
using FCS_AlterraHub.Helpers;
using FCS_AlterraHub.Mods.FCSPDA.Enums;
using FCS_AlterraHub.Mods.FCSPDA.Mono;
using FCS_AlterraHub.Registration;
using FCS_HomeSolutions.Mods.QuantumTeleporter.Enumerators;
using FCS_HomeSolutions.Mods.QuantumTeleporter.Interface;
using FCS_HomeSolutions.Mods.QuantumTeleporter.Mono;
using FCS_HomeSolutions.Mods.QuantumTeleporter.Spawnables;
using FCSCommon.Utilities;
using HarmonyLib;

namespace AdaptiveTeleportingCosts.Patches
{
    [HarmonyPatch(typeof(QuantumPowerBankSpawnable))]
    internal class QuantumPowerBankSpawnablePatches
    {
        [HarmonyPatch(nameof(QuantumPowerBankSpawnable.Teleport))]
        public static bool Prefix(string id, ref bool __result)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                var device = FCSAlterraHubService.PublicAPI.FindDevice(id);

                if (device.Value != null)
                {
                    var destinationTeleporter = device.Value.gameObject.GetComponent<IQuantumTeleporter>();

                    if (!destinationTeleporter.IsOperational)
                    {
                        QuickLogger.ModMessage("This teleporter is not Operational.");
                    }

                    if (device.Value.Manager.IsSame(Player.main.GetCurrentSub()))
                    {
                        QuickLogger.ModMessage("Cannot teleport to the same base.");
                    }

                    var powerBankTechType = "QuantumPowerBank".ToTechType();

                    if (PlayerInteractionHelper.HasItem(powerBankTechType))
                    {
                        var powerBanks = PlayerInteractionHelper.GetItemsOnPlayer(powerBankTechType);

                        foreach (InventoryItem bank in powerBanks)
                        {
                            //Get power bank controller
                            var bankController = bank.item.gameObject.GetComponent<QuantumPowerBankController>();

                            //Check if the power bank has enough power
                            var cost = TeleportUtils.GetTeleportCost(bankController, destinationTeleporter);

                            if (bankController.PowerManager.PowerAvailable() < cost) continue;

                            //Check if a valid destination
                            if (!AdaptiveValidation(destinationTeleporter, cost, out string result))
                            {
                                QuickLogger.ModMessage(result);
                                __result = false; 
                                return false;
                            }

                            FCSPDAController.Main.GoToPage(PDAPages.Home);
                            FCSPDAController.Main.Close();
                            TeleportManager.TeleportPlayer(bankController, destinationTeleporter, Player.main.IsPiloting() ? QTTeleportTypes.Vehicle : QTTeleportTypes.Global);
                            QuickLogger.ModMessage("Teleport SuccessFull");
                            __result =  true;
                            return false;
                        }

                        QuickLogger.ModMessage("Power bank doesn't have enough power for teleporting");
                        __result = false; 
                        return false;
                    }

                    QuickLogger.ModMessage("Requires a Quantum Power Bank on person");
                    __result = false; 
                    return false;
                }

                QuickLogger.ModMessage($"Failed to find teleporter with the ID of : {id}");
                __result = false; 
                return false;
            }

            __result = false; 
            return false;
        }
        public static bool AdaptiveValidation(IQuantumTeleporter destinationTeleporter, float cost, out string result)
        {
            result = string.Empty;

            //Check is player is in a vehicle. If so make sure the destination is not in a base
            if (Player.main.IsPiloting())
            {
                if (destinationTeleporter.IsInside())
                {
                    result = "Cant teleport a vehicle to this teleporter";
                    return false;
                }
            }

            //Check if destination is operational
            if (!destinationTeleporter.IsOperational)
            {
                result = "Destination Teleporter is not operational";
                return false;
            }

            //Check if there is enough power at destination
            if (destinationTeleporter.PowerManager.PowerAvailable() < cost)
            {
                result = "Target destination doesn't have enough power to teleport";
                return false;
            }

            return true;
        }
    }
}
