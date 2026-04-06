using HarmonyLib;
using Snomod.Prefabs;

namespace Snomod.Patches;

internal class OldSocksCommissionCompatPatch
{
    public static void Patch()
    {
        var type = AccessTools.TypeByName("Socksfor1Mod.Patches.Creature_Start");
        if(type == null)
        {
            QMod.Logger.LogError("Failed to find type for old socks patch, but it was listed in chainloader!");
            return;
        }
        var meth = AccessTools.Method(type, "Postfix");
        if(meth == null)
        {
            QMod.Logger.LogError("Failed to find method for old socks patch, but it was listed in chainloader!");
            return;
        }

        var harm = new Harmony("EldritchCarMaker.OldSocksCompat");
        harm.Patch(meth, prefix: new HarmonyMethod(typeof(OldSocksCommissionCompatPatch).GetMethod(nameof(Prefix))));

        QMod.Logger.LogInfo("Old socks patch applied! They still stink!");
    }

    public static bool Prefix(Creature __0)//__0 is another way to access specific arguments in harmony patches. Normally you can use the name of the argument, but in this case the name of the original argument is __instance. It likely works if I simply use ____instance, but to add more clarity it's best to use __0
    {
        if(CraftData.GetTechType(__0.gameObject) == Amogus.TT)
        {
            return false;
        }
        return true;
    }
}
