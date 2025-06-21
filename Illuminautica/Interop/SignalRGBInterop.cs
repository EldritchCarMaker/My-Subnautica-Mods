using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Illuminautica.Interop;

internal class SignalRGBInterop : IInteropHandler
{
    private const string APIURL = "http://localhost:16034/effect/apply/Solid%20Color?color={arg0}&-silentlaunch-";

    public IEnumerator SetColor(Color color)
    {
        var hexColor = ColorUtility.ToHtmlStringRGB(color);

        UnityWebRequest www = UnityWebRequest.Post(string.Format(APIURL, hexColor), "");

        yield return www.SendWebRequest();

        if (!string.IsNullOrEmpty(www.error))
        {
            Plugin.logger.LogError(www.error);
            yield break;
        }

        Plugin.logger.LogMessage($"Sent lighting event to SignalRGB! Set color to {hexColor}");
    }
}
