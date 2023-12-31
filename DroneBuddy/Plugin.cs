using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Reflection;

namespace DroneBuddy
{
    [BepInPlugin("EldritchCarMaker.DroneBuddy", "Drone Buddy", "0.0.1")]
    [BepInDependency("com.snmodding.nautilus")]
    public class Plugin : BaseUnityPlugin
    {
        public new static ManualLogSource Logger { get; private set; }

        private static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();

        private void Awake()
        {
            // set project-scoped logger instance
            Logger = base.Logger;

            // register harmony patches, if there are any
            Harmony.CreateAndPatchAll(Assembly);
            Logger.LogInfo($"Drone buddy loaded! :)");
        }
    }
}