using System.Collections;
using UnityEngine;

namespace Illuminautica.Interop;

internal static class InteropManager
{
    private static IInteropHandler interopHandler;
    private static Color targetColor;
    private static Color sourceColor;
    private static float progress;
    private static float lerpDuration;

    internal static void SetUpManager()
    {
        //For now, since we don't have any other interop handlers anyway
        SetInteropHandler(new SignalRGBInterop());
        targetColor = Color.blue;
        sourceColor = Color.blue;
        progress = 1;
        UWE.CoroutineHost.StartCoroutine(SetCurrentColorAsync());
    }
    //This should be the primary setter, things should NOT set the interop handler field directly
    //This is for future proofing in case a future interop handler requires specific handling
    //ie; connection and disconnection between itself and an external app
    //In fact this maybe should become async for that very reason, but for now it's fine, as long as it's internal those changes can be made later
    internal static void SetInteropHandler(IInteropHandler interopHandler)
    {
        if(InteropManager.interopHandler == interopHandler)
        {
            return;
        }

        InteropManager.interopHandler = interopHandler;
    }

    private static IEnumerator SetCurrentColorAsync()
    {
        while(true)
        {
            yield return null;//Just passes the current frame

            if (progress >= 1)
                continue;//Skips the color setting later down, just passes. We have nothing to do



            Color color = Color.Lerp(sourceColor, targetColor, progress);
            yield return interopHandler.SetColor(color);
            progress += Time.deltaTime / lerpDuration;//Theoretically this could be a divide by 0 error
            //But progress should always be set to 1 if lerpDuration is 0 anyway
            //So in practice should never happen
        }
    }

    internal static void SetCurrentColor(Color color, float lerpDuration)
    {
        sourceColor = lerpDuration > 0 ? Color.Lerp(sourceColor, targetColor, progress) : sourceColor;
        targetColor = color;
        progress = lerpDuration > 0 ? 0 : 1;//full progress if 0 duration
        InteropManager.lerpDuration = lerpDuration;
    }
}
