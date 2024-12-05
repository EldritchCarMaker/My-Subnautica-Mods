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
using MoreCyclopsUpgrades.API;

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


            var module = new CyclopsRemoteControlModule();
#if !SN1 
            module.Info.WithIcon(module.GetItemSprite());
#endif
            module.Patch();
            
            MCUServices.Register.CyclopsUpgradeHandler((SubRoot cyclops) =>
            {
                return new CyclopsRemoteControlHandler(module.TechType, cyclops);
            });
            new TeleportVehicleModule().Patch();
            //new RemoteControlHudChip().Patch();Tbh with you I got no clue why the hell this even exists, it does nothing

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
        [Keybind("Remote Control Key", Tooltip = "Press this key while holding a remote control and have a vehicle with a remote control module equipped to control it remotely")]
        public KeyCode ControlKey = KeyCode.J;

        [Keybind("Cyclops Remote Control Key", Tooltip = "Press this key while holding a remote control and have a cyclops with a remote control module equipped to control it remotely")]
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

        [Toggle("Must Be In Base", Tooltip = "Are you allowed to remote control while in non-safe places (not in a base) or not?")]
        public bool MustBeInBase = true;

        public int remoteStorageWidth = 2;
        public int remoteStorageHeight = 2;
    }
}