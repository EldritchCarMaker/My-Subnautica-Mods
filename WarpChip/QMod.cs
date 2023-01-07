using System.Reflection;
using HarmonyLib;
#if !SN2
using QModManager.API.ModLoading;
using Logger = QModManager.Utility.Logger;
#else
using BepInEx;
#endif
using SMLHelper.V2.Json;
using SMLHelper.V2.Options.Attributes;
using SMLHelper.V2.Handlers;
using UnityEngine;
using WarpChip.Items;

namespace WarpChip
{
#if !SN2
    [QModCore]
    public static class QMod
    {
#else
    [BepInPlugin("EldritchCarMaker.WarpChip", "Warp Chip", "1.4.0")]
    public class QMod : BaseUnityPlugin
    {
#endif
        public static Config config { get; } = OptionsPanelHandler.Main.RegisterModOptions<Config>();
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

            WarpChipItem warpChipItem = new WarpChipItem();
            warpChipItem.Patch();

            new UltimateWarpChip().Patch();

            new TelePingBeacon().Patch();

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
