using System.Reflection;
using HarmonyLib;
using QModManager.API.ModLoading;
using Logger = QModManager.Utility.Logger;

using SMLHelper.V2.Json;
using SMLHelper.V2.Options.Attributes;
using SMLHelper.V2.Handlers;
using UnityEngine;

namespace WarpChip
{
    [QModCore]
    public static class QMod
    {
        internal static Config config { get; } = OptionsPanelHandler.Main.RegisterModOptions<Config>();
        [QModPatch]
        public static void Patch()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var CyclopsLockers = ($"EldritchCarMaker_{assembly.GetName().Name}");
            Logger.Log(Logger.Level.Info, $"Patching {CyclopsLockers}");
            Harmony harmony = new Harmony(CyclopsLockers);
            harmony.PatchAll(assembly);

            WarpChipItem warpChipItem = new WarpChipItem();
            warpChipItem.Patch();

            new UltimateWarpChip().Patch();

            Logger.Log(Logger.Level.Info, "Patched successfully!");
        }
    }
    [Menu("Warp Chip")]
    public class Config : ConfigFile
    {
        [Keybind("Warp Key", Tooltip = "Press this key while you have a warp chip equipped to warp forward slightly")]
        public KeyCode ControlKey = KeyCode.J;

        [Toggle("Can Teleport To Lifepod", Tooltip = "Whether the chip can warp you to your lifepod if it can't find a valid base to warp to")]
        public bool CanTeleportToLifepod = false;

        [Toggle("Teleport To Main Lifepod", Tooltip = "By default, can only teleport to lifepod if you have already entered it this play session. If this config is true, you teleport to it regardless")]
        public bool TeleportToLifepodOnOldSave = false;

        public int MaxDistanceToTeleportToLifepod = 0;

        public int DefaultWarpDistanceOutside = 15;
        public int DefaultWarpDistanceInside = 10;
        public float DefaultWarpCooldown = 5;
    }
}
