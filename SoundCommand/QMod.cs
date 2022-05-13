using System.Reflection;
using HarmonyLib;
using QModManager.API.ModLoading;
using Logger = QModManager.Utility.Logger;

using SMLHelper.V2.Json;
using SMLHelper.V2.Options.Attributes;
using SMLHelper.V2.Handlers;
using UnityEngine;
using System.Collections.Generic;

namespace WarpChip
{
    [QModCore]
    public static class QMod
    {
        public static List<FMOD_CustomLoopingEmitter> playingSounds = new List<FMOD_CustomLoopingEmitter>();

        [QModPatch]
        public static void Patch()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var CyclopsLockers = ($"Nagorogan_{assembly.GetName().Name}");
            Logger.Log(Logger.Level.Info, $"Patching {CyclopsLockers}");
            Harmony harmony = new Harmony(CyclopsLockers);

            ConsoleCommandsHandler.Main.RegisterConsoleCommand("PlaySound", typeof(QMod), nameof(PlaySound));
            ConsoleCommandsHandler.Main.RegisterConsoleCommand("PlayLoopSound", typeof(QMod), nameof(PlayLoopSound));
            ConsoleCommandsHandler.Main.RegisterConsoleCommand("StopLoopSound", typeof(QMod), nameof(StopLoopSound));

            Logger.Log(Logger.Level.Info, "Patched successfully!");
        }
        public static void PlaySound(string n)
        {
            Logger.Log(Logger.Level.Info, "Playing sound", null, true); 
            if (n != null)
            {
                FMODAsset sound = Utility.GetFmodAsset(n);
                if (sound != null) Utils.PlayFMODAsset(sound, Player.main.transform);
            }
        }
        public static void PlayLoopSound(string n)
        {
            Logger.Log(Logger.Level.Info, "Starting loop", null, true); 
            if (n != null)
            {
                FMODAsset sound = Utility.GetFmodAsset(n);

                if (sound == null) return;

                FMOD_CustomLoopingEmitter loop = AddLoopingEmitter(sound);

                if (loop != null)
                {
                    playingSounds.Add(loop);
                    loop.Play();
                }
            }
        }
        public static void StopLoopSound(string n)
        {
            Logger.Log(Logger.Level.Info, "Stopping loop", null, true); 
            if (n != null)
            {
                FMODAsset sound = Utility.GetFmodAsset(n);

                if (sound == null) return;
                
                foreach(FMOD_CustomLoopingEmitter loop in playingSounds)
                {
                    if(loop.asset == sound) loop.Stop();
                }
            }
        }

        private static FMOD_CustomLoopingEmitter AddLoopingEmitter(FMODAsset asset)
        {
            var emitter = Player.main.gameObject.AddComponent<FMOD_CustomLoopingEmitter>();
            emitter.SetAsset(asset);
            emitter.followParent = true;
            emitter.restartOnPlay = false;
            return emitter;
        }
    }
}
