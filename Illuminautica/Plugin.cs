using BepInEx;
using BepInEx.Logging;

namespace Illuminautica;

[BepInPlugin("EldritchCarMaker.Illuminautica", "Illuminautica", "1.0")]
internal class Plugin : BaseUnityPlugin
{
    internal static ManualLogSource logger;


#pragma warning disable IDE0051 // Remove unused private members (It's not unused)
    private void Awake()
#pragma warning restore IDE0051 // Remove unused private members
    {
        logger = Logger;

        
    }
}
