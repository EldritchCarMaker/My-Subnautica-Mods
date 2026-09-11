using System;
using System.IO;
using UnityEngine;
using UnityEngine.Video;

namespace MainMenuVideo;

internal class VideoSettingsManager : MonoBehaviour
{
    //The news feed is initialized later, so we patch that and apply opacity on it separately. Once it's initialized, the normal settings managers handle it properly
    internal static float currentMenuOpacity = 1;
    private VideoSettings settings;
    private VideoPlayer videoPlayer;
    internal void Initialize(string videoFile, string fileName, VideoPlayer videoPlayer)
    {
        this.videoPlayer = videoPlayer;
        var settingsFilePath = Path.Combine(Path.GetDirectoryName(videoFile), $"{fileName}.json");
        if (!File.Exists(settingsFilePath))
        {
            return;
        }

        var json = File.ReadAllText(settingsFilePath);

        settings = JsonUtility.FromJson<VideoSettings>(json);
    }

    private void OnEnable()
    {
        if(settings == null) return;

        videoPlayer.SetDirectAudioVolume(0, settings.Volume);
        videoPlayer.isLooping = settings.Loop;
        if (settings.MuteGame) MainMenuMusic.Stop();
        videoPlayer.targetCameraAlpha = settings.Opacity;
        foreach(var canRend in uGUI_MainMenu.main.GetComponentsInChildren<CanvasRenderer>(true))
            canRend.SetAlpha(settings.MenuOpacity);//Only one canvas renderer has an alpha less than one, but I don't care about that tiny X. So it's easy and safe to set them all
        currentMenuOpacity = settings.MenuOpacity;
    }

    private void OnDisable()
    {
        if (settings == null) return;
        if (settings.MuteGame) MainMenuMusic.Play();
        foreach (var canRend in uGUI_MainMenu.main.GetComponentsInChildren<CanvasRenderer>(true))
            canRend.SetAlpha(1);//Again, one renderer should have 0.5 alpha but idc
        currentMenuOpacity = 1;
    }


    [Serializable]
    internal class VideoSettings
    {
        public float Volume = 1;
        public bool Loop = true;
        public bool MuteGame = true;
        public float Opacity = 1;
        public float MenuOpacity = 0.75f;
    }
}
