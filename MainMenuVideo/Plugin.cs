using System.Collections.Generic;
using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Nautilus.Handlers.TitleScreen;
using UnityEngine;
using UnityEngine.Video;

namespace MainMenuVideo;

[BepInPlugin("EldritchCarMaker.MainMenuVideo", "Main Menu Video", "1.0.0")]
internal class Plugin : BaseUnityPlugin
{
    public static AssetBundle assertBuundoggle;
    public static string modPath = Path.Combine(Paths.PluginPath, "MainMenuVideo");
    public static readonly List<string> foundFileNames = new();
    public static ManualLogSource logSource;
    private void Awake()
    {
        logSource = Logger;
        assertBuundoggle = AssetBundle.LoadFromFile(Path.Combine(modPath, "mainmenuvideo"));

        var harm = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
        

        var videoFiles = Directory.GetFiles(Path.Combine(Plugin.modPath, "Videos"), "*", SearchOption.AllDirectories);

        foreach(var videoFile in videoFiles)
        {
            if (Path.GetExtension(videoFile).ToLower() == ".json") continue;//Unity handles most video formats so it's easier to blacklist than whitelist

            var prefab = Plugin.assertBuundoggle.LoadAsset<GameObject>("VideoPlayerPrefab");
            var fileName = Path.GetFileNameWithoutExtension(videoFile);
            foundFileNames.Add(fileName);

            var mainMenuVideoCreator = new WorldObjectTitleAddon(() =>
            {
                var instance = GameObject.Instantiate(prefab);

                var videoPlayer = instance.GetComponentInChildren<VideoPlayer>();
                videoPlayer.url = videoFile;
                var targetCamera = Camera.allCameras[1];//I fucking hate this game why do they have 4 different cameras in the main menu (MainCamera.camera and Camera.main both point to the UI camera, which is the third camera for some reason)
                videoPlayer.targetCamera = targetCamera;
                videoPlayer.Play();

                instance.AddComponent<VideoSettingsManager>().Initialize(videoFile, fileName, videoPlayer);

                return instance;
            });
            var data = new TitleScreenHandler.CustomTitleData(fileName, mainMenuVideoCreator);

            TitleScreenHandler.RegisterTitleScreenObject("{fileName}", data);
        }


        var nautilusPatcher = AccessTools.TypeByName("Nautilus.Patchers.MainMenuPatcher");
        var nautilusMethod = nautilusPatcher.GetMethod("CreateSelectionUI", AccessTools.all);
        var transpiler = AccessTools.DeclaredMethod(typeof(NautilusTranspiler), nameof(NautilusTranspiler.Transpiler));

        harm.Patch(nautilusMethod, transpiler: new HarmonyMethod(transpiler));


        Logger.LogInfo("Main Menu Video plugin loaded.");
    }
}