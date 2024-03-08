using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Collections;
using System.Reflection;
using UnityEngine;

namespace CloudyWithAChanceOfTorpedoes
{
    [BepInPlugin("EldritchCarMaker.CloudyWithAChanceOfTorpedoes", "Cloudy With A Chance Of Torpedoes", "0.0.1")]
    [BepInDependency("com.snmodding.nautilus")]
    public class Plugin : BaseUnityPlugin
    {
        public new static ManualLogSource Logger { get; private set; }

        private static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();
        private void Awake()
        {
            Logger = base.Logger;

            Harmony.CreateAndPatchAll(Assembly);
            ChanceOfTorpedoes.ForecastWeather();

            Logger.LogInfo($"Weather report: Cloudy, with a chance of torpedoes");
        }
    }
}