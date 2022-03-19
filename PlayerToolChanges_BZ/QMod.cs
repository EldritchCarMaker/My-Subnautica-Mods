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

namespace RepairToolChanges_SN
{
    [QModCore]
    public static class QMod
    {
        
        internal static Config config { get; } = OptionsPanelHandler.Main.RegisterModOptions<Config>();
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
    [Menu("Player Tool Changes")]
    public class Config : ConfigFile
    {   
        //repair tool config
        [Slider("Repair Tool Repair Rate", Format = "{0:F2}", Min = 0.0F, Max = 50.0f, DefaultValue = 1.0F, Step = 0.5F, Tooltip = "Multiplies how much the repair tool repairs. Default is 1")]
        public float repairToolRepairRate = 1.0F;
        [Slider("Repair Tool Energy Cost", Format = "{0:F2}", Min = 0.0F, Max = 50.0F, DefaultValue = 1.0F, Step = 0.5F, Tooltip = "Multiplies how much energy the repair tool consumes. Default is 1")]
        public float repairToolEnergyCost = 1.0F;
        //laser cutter config
        [Slider("Laser Cutter cutting speed", Format = "{0:F2}", Min = 0.0F, Max = 50.0F, DefaultValue = 1.0F, Step = 0.5F, Tooltip = "Multiplies how fast the laser cutter will cut. Default is 1")]
        public float laserCutterCutRate = 1.0F;
        [Slider("Laser Cutter Energy Cost", Format = "{0:F2}", Min = 0.0F, Max = 50.0F, DefaultValue = 1.0F, Step = 0.5F, Tooltip = "Multiplies how much energy the lase cutter consumes. Default is 1")]
        public float laserCutterEnergyCost = 1.0F;
        //Knife Config
        [Slider("knife damage", Format = "{0:F2}", Min = 0.0F, Max = 50.0F, DefaultValue = 1.0F, Step = 0.5F, Tooltip = "Multiplies how much damage the knife will do. Default is 1")]
        public float knifeDamage = 1.0f;
        [Slider("knife distance", Format = "{0:F2}", Min = 0.0F, Max = 50.0F, DefaultValue = 1.0F, Step = 0.5F, Tooltip = "Multiplies how far the knife can reach. Default is 1")]
        public float knifeRange = 1.0f;
        //propulsion cannon config
        [Slider("Propulsion Cannon Pickup Distance", Format = "{0:F2}", Min = 0.0F, Max = 50.0F, DefaultValue = 1.0F, Step = 0.5F, Tooltip = "Multiplies how far you can pick things up from with the propulsion cannon. Default is 1")]
        public float propCannonPickupDist = 1.0F;
        [Slider("Propulsion Cannon Shoot Force", Format = "{0:F2}", Min = 0.0F, Max = 50.0F, DefaultValue = 1.0F, Step = 0.5F, Tooltip = "Multiplies how much force the propulsion cannon uses when it shoots items. Default is 1")]
        public float propCannonShootForce = 1.0F;
        [Slider("Propulsion cannon Energy Cost", Format = "{0:F2}", Min = 0.0F, Max = 50.0F, DefaultValue = 1.0F, Step = 0.5F, Tooltip = "Multiplies how much energy the propulsion cannon consumes. Default is 1")]
        public float propCannonEnergyCost = 1.0F;
        [Slider("Propulsion cannon Shoot Energy Cost", Format = "{0:F2}", Min = 0.0F, Max = 50.0F, DefaultValue = 1.0F, Step = 0.5F, Tooltip = "Multiplies how much energy the propulsion cannon consumes per shot. Default is 1")]
        public float propCannonShootEnergyCost = 1.0F;
        [Slider("Propulsion cannon max mass", Format = "{0:F2}", Min = 0.0F, Max = 50.0F, DefaultValue = 1.0F, Step = 0.5F, Tooltip = "Multiplies the max mass the propulsion cannon can pick up. Default is 1")]
        public float propCannonMaxMass = 1.0F;
        [Slider("Propulsion cannon attraction force", Format = "{0:F2}", Min = 0.0F, Max = 50.0F, DefaultValue = 1.0F, Step = 0.5F, Tooltip = "Multiplies how much force the propulsion cannon uses to bring things to it. Default is 1")]
        public float propCannonAttractionForce = 1.0f;
        [Slider("Propulsion cannon mass scaling", Format = "{0:F2}", Min = 0.0F, Max = 50.0F, DefaultValue = 1.0F, Step = 0.5F, Tooltip = "Multiplies how much the prop cannon scales the amount of force used based on the mass of the object carried.. Default is 1")]
        public float propCannonMassScalingFactorMultiplier = 1.0f;
        [Toggle("Propulsion cannon target override", Tooltip = "WARNING! VERY BUGGY! DO NOT USE WITHOUT SAVING. Allow the propulsion cannon to target anything including objects marked as immune to the propulsion cannon")]
        public bool targetOverride = false;
        //stasis rifle config
        [Slider("Stasis rifle charge speed", Format = "{0:F2}", Min = 0.0F, Max = 50.0F, DefaultValue = 1.0F, Step = 0.5F, Tooltip = "Multiplies how fast the stasis rifle charges. Takes 3 seconds to charge at default, setting this to 2 turns that to 1.5 seconds. Default is 1")]
        public float stasisRifleChargeRate = 1.0F;
        [Slider("Stasis rifle energy cost", Format = "{0:F2}", Min = 0.0F, Max = 50.0F, DefaultValue = 1.0F, Step = 0.5F, Tooltip = "Multiplies how much energy drain the stasis rifle has. Default is 1")]
        public float stasisRifleEnergyCost = 1.0F;
        [Slider("Stasis rifle bubble radius", Format = "{0:F2}", Min = 0.0F, Max = 50.0F, DefaultValue = 1.0F, Step = 0.5F, Tooltip = "Multiplies how big the stasis bubble is. Default is 1")]
        public float stasisRifleBubbleRadius = 1.0F;
        [Slider("Stasis rifle bubble duration", Format = "{0:F2}", Min = 0.0F, Max = 50.0F, DefaultValue = 1.0F, Step = 0.5F, Tooltip = "Multiplies how long the stasis bubble lasts. Default is 1")]
        public float stasisRifleBubbleDuration = 1.0F;
        //scanner config
        [Slider("Scan duration", Format = "{0:F2}", Min = 0.0F, Max = 4.0F, DefaultValue = 2.0F, Step = 0.1F, Tooltip = "Amount of time in seconds to scan something with the scanner tool. Default changes based on the item. Changing this value to anything other than 2 will cause every item to take exactly this amount of seconds to scan. Leave at 2 and the scan time will continue to adjust based on the item")]
        public float ScanDuration = 2.0F;
        [Slider("Scanner Energy cost", Format = "{0:F2}", Min = 0.0F, Max = 50.0F, DefaultValue = 1.0F, Step = 0.5F, Tooltip = "Multiplies how much energy drain the scanner tool has. Default is 1")]
        public float scannerEnergyCost = 1.0F;
    }
}