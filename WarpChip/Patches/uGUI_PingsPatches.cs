using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using WarpChip.Monobehaviours;

namespace WarpChip.Patches
{
    [HarmonyPatch(typeof(uGUI_Pings))]
    internal class uGUI_PingsPatches
    {
        public static Dictionary<uGUI_Ping, bool> pings = new Dictionary<uGUI_Ping, bool>();

        [HarmonyPatch(nameof(uGUI_Pings.OnAdd))]
        public static bool Prefix(uGUI_Pings __instance, PingInstance instance)
        {
            if (!instance.TryGetComponent<TelePingInstance>(out var telePing)) return true;

            uGUI_Ping uGUI_Ping = __instance.poolPings.Get();
            uGUI_Ping.Initialize();
            uGUI_Ping.SetVisible(instance.visible);
            uGUI_Ping.SetColor(PingManager.colorOptions[instance.colorIndex]);
            uGUI_Ping.SetIcon(SpriteManager.Get(SpriteManager.Group.Pings, PingManager.sCachedPingTypeStrings.Get(instance.pingType)));
            uGUI_Ping.SetLabel(instance.GetLabel());
            uGUI_Ping.SetIconAlpha(0f);
            uGUI_Ping.SetTextAlpha(0f);
            __instance.pings.Add(instance.Id, uGUI_Ping);

            telePing.ping = uGUI_Ping;

            return false;
        }
    }
}
