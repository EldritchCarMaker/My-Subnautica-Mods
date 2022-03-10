using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using HarmonyLib;
using QModManager.API.ModLoading;
using Logger = QModManager.Utility.Logger;

using SMLHelper.V2.Json;
using SMLHelper.V2.Options.Attributes;
using SMLHelper.V2.Handlers;

namespace FasterSnowFoxCharging
{
    [QModCore]
    public static class QMod
    {
        
        internal static Config Config { get; } = OptionsPanelHandler.Main.RegisterModOptions<Config>();
        [QModPatch]
        public static void Patch()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var testMod = ($"Nagorogan_{assembly.GetName().Name}");
            Logger.Log(Logger.Level.Info, $"Patching {testMod}");
            Harmony harmony = new Harmony(testMod);
            harmony.PatchAll(assembly);
            Logger.Log(Logger.Level.Info, "Patched successfully!");
        }
    }
    [Menu("Faster Snow Fox Charging")]
    public class Config : ConfigFile
    {
        [Slider("Dock Charging Speed", Format = "{0:F2}", Min = 0.0F, Max = 100.0F, DefaultValue = 1.0F, Step = 0.5F, Tooltip = "If using percentages, this value is the percent of snowfox battery charged per charge tick. If using flat values, this value is the amount of battery charged per tick. Default is 1")]
        public float dockChargeSpeed = 1.0f;
        [Slider("Dock Repair Speed", Format = "{0:F2}", Min = 0.0F, Max = 100.0F, DefaultValue = 5.0F, Step = 0.5F, Tooltip = "If using percentages, this value is the percent of snowfox health repaired per repair tick. If using flat values, this value is the amount of health repaired per tick. Default is 5")]
        public float dockRepairSpeed = 5.0f;
        [Toggle("Dock uses percentage", Tooltip ="Toggles whether the snowfox dock will charge and repair using a percentage of the snowfox's values per tick or a flat value. Checked means the dock will use a percentage while unchecked means it will use flat values")]
        public bool dockUsesPercentage = true;
    }
}