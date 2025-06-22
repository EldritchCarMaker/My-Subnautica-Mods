using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace Illuminautica.Interop;

internal class SignalRGBInterop : IInteropHandler
{
    private static string ArgumentsStart = " --url=effect/apply/Solid%20Color?color="; 
    private static string ArgumentsEnd = "&-silentlaunch-";
    private static string GetArgs(string hexColor) => ArgumentsStart + hexColor + ArgumentsEnd;

    public IEnumerator SetColor(Color color)
    {
        var hexColor = ColorUtility.ToHtmlStringRGB(color);

        ProcessStartInfo start = new ProcessStartInfo();
        start.Arguments = GetArgs(hexColor);
        start.FileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "VortxEngine", "SignalRgbLauncher.exe");
        start.WindowStyle = ProcessWindowStyle.Hidden;
        Process.Start(start);
        yield return new WaitForSeconds(0.5f);
    }
}
