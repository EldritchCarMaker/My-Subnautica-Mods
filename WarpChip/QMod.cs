using System.Reflection;
using HarmonyLib;
#if SN1
using QModManager.API.ModLoading;
using Logger = QModManager.Utility.Logger;
using SMLHelper.V2.Json;
using SMLHelper.V2.Options.Attributes;
using SMLHelper.V2.Handlers;
#else
using Nautilus.Json;
using Nautilus.Options.Attributes;
using Nautilus.Handlers;
#endif
using System.IO;
using UnityEngine;
using BepInEx;
using WarpChip.Items;

namespace WarpChip
{
#if !SN2
    [QModCore]
    public static class QMod
    {
        public static Config config { get; } = OptionsPanelHandler.Main.RegisterModOptions<Config>();
#else
    [BepInPlugin("EldritchCarMaker.WarpChip", "Warp Chip", "1.4.1")]
    [BepInDependency("EldritchCarMaker.EquippableItemIcons", "1.4.0")]
    public class QMod : BaseUnityPlugin
    {
        public static Config config { get; } = OptionsPanelHandler.RegisterModOptions<Config>();
#endif
#if !SN2
        [QModPatch]
        public static void Patch()
        {
#else
        public void Awake()
        {
#endif
            var assembly = Assembly.GetExecutingAssembly();
            var name = ($"EldritchCarMaker_{assembly.GetName().Name}");
#if !SN2
            Logger.Log(Logger.Level.Info, $"Patching {name}");
#else
            Logger.LogInfo($"Patching {name}");
#endif
            Harmony harmony = new Harmony(name);
            harmony.PatchAll(assembly);

            WarpChipItem.Patch();

            UltimateWarpChip.Patch();

            TelePingBeacon.Patch();

            TelePingVehicleModule.Patch();

#if !SN2
            Logger.Log(Logger.Level.Info, "Patched successfully!");
#else
            Logger.LogInfo("Patched successfully!");
#endif
        }
    }
    [Menu("Warp Chip")]
    public class Config : ConfigFile
    {
        [Keybind("Warp Key", Tooltip = "Press this key while you have a warp chip equipped to warp forward slightly")]
        public KeyCode ControlKey = KeyCode.J;
#if SN
        [Toggle("Can Teleport To Lifepod", Tooltip = "Whether the chip can warp you to your lifepod if it can't find a valid base to warp to")]
        public bool CanTeleportToLifepod = false;

        [Toggle("Teleport To Main Lifepod", Tooltip = "By default, can only teleport to lifepod if you have already entered it this play session. If this config is true, you teleport to it regardless")]
        public bool TeleportToLifepodOnOldSave = false;

        public int MaxDistanceToTeleportToLifepod = 0;
#endif
        public int DefaultWarpDistanceOutside = 15;
        public int DefaultWarpDistanceInside = 10;
        public float DefaultWarpCooldown = 5;
    }
}
