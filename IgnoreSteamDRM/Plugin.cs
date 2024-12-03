using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Nautilus.Handlers;
using System.Reflection;

namespace IgnoreSteamDRM
{
    [BepInPlugin("EldritchCarMaker.IgnoreSteamDRM", "Ignore Steam DRM", "0.0.1")]
    [BepInDependency("com.snmodding.nautilus")]
    public class Plugin : BaseUnityPlugin
    {
        public static new Config Config = OptionsPanelHandler.RegisterModOptions<Config>();
        public new static ManualLogSource Logger { get; private set; }

        private static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();
        private void Awake()
        {
            Logger = base.Logger;

            Harmony.CreateAndPatchAll(Assembly);
            
            Logger.LogInfo($"Ignore Steam DRM loaded!");
        }
    }
}