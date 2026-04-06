using HarmonyLib;
using Sentry;
using Snomod.MonoBehaviours;

namespace Snomod.Patches;

[HarmonyPatch(typeof(Player))]
internal class PlayerDeathPatch
{
    [HarmonyPatch(nameof(Player.OnKill))]
    public static void Prefix(Player __instance)
    {
        FMODUWE.PlayOneShot(MogusSounds.deathSound, __instance.transform.position);
    }
}
