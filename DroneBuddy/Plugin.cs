using BepInEx;
using BepInEx.Logging;
using DroneBuddy.Items;
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
            Logger = base.Logger;

            Harmony.CreateAndPatchAll(Assembly);
            DroneItem.RegisterDrone();
            
            Logger.LogInfo($"Drone buddy loaded! :)");
        }
    }
}