using System.Collections;
using System.IO;
using Steamworks;
using UnityEngine;

namespace IgnoreSteamDRM;

internal class StrippedPlatformServiesSteam : PlatformServices//Why the fuck this aint start with an I
{
    private bool devToolsEnabled;
    private string RichPresenceStr = string.Empty;
    private UserStoragePC userStoragePC;

    public StrippedPlatformServiesSteam() => InitializeSync();//I know using a constructor isnt ideal here but im lazy as fuck so...
    public void InitializeSync()
    {
        string fullName = Directory.GetParent(Application.dataPath).FullName;
        string savePath = Path.Combine(fullName, "SNAppData/SavedGames");
        userStoragePC = new UserStoragePC(savePath);
    }


    public bool CanAccessServers() => true;

    public bool CanAccessUGC() => true;

    public void DeregisterOnQuitBehaviour(IOnQuitBehaviour behaviour) { }

    public int GetActiveController() => -1;

    public DisplayOperationMode GetCurrentDisplayOperationMode() => DisplayOperationMode.Default;

    public string GetDefaultNewsUrl() => "https://subnautica.unknownworlds.com/api/news/pc-new";

    public float GetDefaultUiScale(DisplayOperationMode displayOperationMode) => 1f;

    public bool GetDevToolsEnabled() => devToolsEnabled;

    public bool GetDisplayOutOfSpaceMessage() => true;

    public IEconomyItems GetEconomyItems()
    {
        return new StrippedEconomyItems();
    }
    public class StrippedEconomyItems : IEconomyItems
    {
        public bool IsReady => true;

        public string GetItemProperty(TechType techType, string key) => string.Empty;

        public bool HasItem(TechType techType) => false;

        public IEnumerator InitializeAsync() { yield break; }
    }

    public string GetName()
    {
        return Plugin.Config.servicesType switch
        {
            Config.ServicesType.Default => null,
            Config.ServicesType.ActLikeSteam => "Steam",
            Config.ServicesType.ActLikeStrippedSteam => "StrippedSteam",
            _ => null
        };
    }

    public string GetRichPresence() => RichPresenceStr;

    public bool GetSupportsDynamicLogOn() => false;

    public bool GetSupportsSharingScreenshots() => true;

    public bool GetSupportsVirtualKeyboard() => false;

    public string GetUserId() => CSteamID.Nil.ToString();

    public string GetUserMusicPath() => PlatformServicesUtils.GetDesktopUserMusicPath();

    public string GetUserName() => "StrippedSteamPlatformServicesUser";

    public UserStorage GetUserStorage() => userStoragePC;

    public bool IsUserLoggedIn() => true;

    public void LogOffUser() { }

    public PlatformServicesUtils.AsyncOperation LogOnUserAsync(int gamepadIndex) => null;

    public void OpenURL(string url, bool overlay = false) => PlatformServicesUtils.DefaultOpenURL(url);

    public bool ReconnectController(int gamepadIndex) => true;

    public void RegisterOnQuitBehaviour(IOnQuitBehaviour behaviour) { }

    public void ResetAchievements() { }

    public void SetDevToolsEnabled(bool enabled) => devToolsEnabled = enabled;

    public void SetRichPresence(string presenceKey) => RichPresenceStr = presenceKey;

    public void SetUseFastLoadMode(bool useFastLoadMode) { }

    public bool ShareScreenshot(string fileName) => false;

    public void ShowHelp() { }

    public void ShowUGCRestrictionMessageIfNecessary() { }

    public bool ShowVirtualKeyboard(string title, string defaultText, PlatformServicesUtils.VirtualKeyboardFinished callback, int characterLimit = -1) => false;

    public void Shutdown() { }

    public IEnumerator TryEnsureServerAccessAsync(bool onUserInput = false) { yield break; }

    public void UnlockAchievement(GameAchievements.Id id) { }

    public void Update() { }
}
