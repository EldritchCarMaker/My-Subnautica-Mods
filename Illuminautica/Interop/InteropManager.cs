using System.Collections;
using UnityEngine;

namespace Illuminautica.Interop;

internal static class InteropManager
{
    private static IInteropHandler interopHandler;

    internal static void SetUpManager()
    {
        //For now, since we don't have any other interop handlers anyway
        SetInteropHandler(new SignalRGBInterop());
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

    private static IEnumerator SetCurrentColorAsync(Color color)
    {
        yield return interopHandler.SetColor(color);
    }

    internal static void SetCurrentColor(Color color)
    {
        UWE.CoroutineHost.StartCoroutine(SetCurrentColorAsync(color));
    }
}
