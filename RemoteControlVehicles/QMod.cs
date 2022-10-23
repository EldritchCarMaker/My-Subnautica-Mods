using System.Reflection;
using HarmonyLib;
using QModManager.API.ModLoading;
using Logger = QModManager.Utility.Logger;

using SMLHelper.V2.Json;
using SMLHelper.V2.Options.Attributes;
using SMLHelper.V2.Handlers;
using RemoteControlVehicles;
using UnityEngine;
using MoreCyclopsUpgrades.API;
using RemoteControlVehicles.Items;

namespace RemoteControlVehicles
{
    [QModCore]
    public static class QMod
    {
        internal static Config config { get; } = OptionsPanelHandler.Main.RegisterModOptions<Config>();
        [QModPatch]
        public static void Patch()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var af = ($"EldritchCarMaker_{assembly.GetName().Name}");
            Logger.Log(Logger.Level.Info, $"Patching {af}");
            Harmony harmony = new Harmony(af);
            harmony.PatchAll(assembly);

            new TeleportVehicleModule().Patch();
            //new RemoteControlHudChip().Patch();

            var module = new CyclopsRemoteControlModule();
            module.Patch();

            new RemoteControlAurora().Patch();
            new RemoteControlCar().Patch();

            new DroneControlRemote().Patch();

            MCUServices.Register.CyclopsUpgradeHandler((SubRoot cyclops) =>
            {
                return new CyclopsRemoteControlHandler(module.TechType, cyclops);
            });

            Logger.Log(Logger.Level.Info, "Patched successfully!");
        }
    }
    [Menu("Remote Control Vehicles")]
    public class Config : ConfigFile
    {
        [Keybind("Remote Control Key", Tooltip = "Press this key while you have a remote control chip and a vehicle with a remote control module equipped to control it remotely")]
        public KeyCode ControlKey = KeyCode.J;

        [Keybind("Cyclops Remote Control Key", Tooltip = "Press this key while you have a remote control chip and a cyclops with a remote control module equipped to control it remotely")]
        public KeyCode cyclopsControlKey = KeyCode.U;

        [Keybind("Dock/Undock vehicle key", Tooltip = "Press this key while using the remote control aurora and above a dockable vehicle to dock the vehicle, or to undock the current vehicle")]
        public KeyCode dockControlKey = KeyCode.Q;

        [Toggle("Instant Camera Position Correction", Tooltip = "When controlling the aurora or rc car, does the camera snap instantly to the correct position or does it lag behind slightly?")]
        public bool instantCamPosition = false;

        [Toggle("Instant Camera Rotation Correction", Tooltip = "When controlling the aurora or rc car, does the camera snap instantly to the correct rotation or does it lag behind slightly?")]
        public bool instantCamRotation = false;

        [Toggle("Smooth Damp Camera Position Correction", Tooltip = "When controlling the aurora or rc car, decides which method to use to keep the camera close. If this is true, use SmoothDamp, else use Lerp. Incompatible with Instant Camera Position Correction.")]
        public bool smoothDampCamPosition = false;

        /*no smooth damp for rotation currently
        [Toggle("Smooth Damp Camera Rotation Correction", Tooltip = "When controlling the aurora or rc car, does the camera snap instantly to the correct rotation or does it lag behind slightly?")]
        public bool smoothDampCamRotation = false;
        */

        public bool MustBeInBase = true;

        public int remoteStorageWidth = 2;
        public int remoteStorageHeight = 2;
    }
}