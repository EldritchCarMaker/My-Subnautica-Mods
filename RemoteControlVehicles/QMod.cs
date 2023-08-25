using System.Reflection;
using HarmonyLib;
#if SN1
using QModManager.API.ModLoading;
using Logger = QModManager.Utility.Logger;
using SMLHelper.V2.Json;
using SMLHelper.V2.Options.Attributes;
using SMLHelper.V2.Handlers;
using MoreCyclopsUpgrades.API;
using LogLevel = Logger.Level;
#else
using LogLevel = BepInEx.Logging.LogLevel;
using Nautilus.Json;
using Nautilus.Options.Attributes;
using Nautilus.Handlers;
#endif


using RemoteControlVehicles;
using UnityEngine;
using RemoteControlVehicles.Items;
using BepInEx;
using BepInEx.Logging;

namespace RemoteControlVehicles
{
#if SN1
    [QModCore]
    public static class QMod
#else
    [BepInPlugin("EldritchCarMaker.RemoteControlVehicles", "Remote Control Vehicles", "1.0.0")]
    public class QMod : BaseUnityPlugin
    {
        public static ManualLogSource Logger;
        public void Awake()
        {
            Logger = base.Logger;
            Patch();
        }
#endif
#if SN1
        [QModPatch]
#endif
        public static void Patch()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var af = ($"EldritchCarMaker_{assembly.GetName().Name}");

            Logger.Log(LogLevel.Info, $"Patching {af}");
            Harmony harmony = new Harmony(af);
            harmony.PatchAll(assembly);


#if SN1
            var module = new CyclopsRemoteControlModule();
            module.Patch();
            
            MCUServices.Register.CyclopsUpgradeHandler((SubRoot cyclops) =>
            {
                return new CyclopsRemoteControlHandler(module.TechType, cyclops);
            });
            TeleportVehicleModule.Patch();
            //new RemoteControlHudChip().Patch();
#endif

            RemoteControlAurora.Patch();
            RemoteControlCar.Patch();

            DroneControlRemote.Patch();


            Logger.Log(LogLevel.Info, "Patched successfully!");
        }

#if SN1
        internal static Config config { get; } = OptionsPanelHandler.Main.RegisterModOptions<Config>();
#else
        internal static Config config { get; } = OptionsPanelHandler.RegisterModOptions<Config>();
#endif
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