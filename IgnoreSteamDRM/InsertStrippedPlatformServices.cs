using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace IgnoreSteamDRM;

[HarmonyPatch(typeof(PlatformUtils), nameof(PlatformUtils.PlatformInitAsync), MethodType.Enumerator)]
internal class InsertStrippedPlatformServices
{
    internal static bool useStrippedServices = false;

    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var matcher = new CodeMatcher(instructions);

        matcher.MatchForward(false, 
            new CodeMatch(inst => inst.operand is FieldInfo info && info.Name.Contains("steamServices")), 
            new CodeMatch(inst => inst.opcode == OpCodes.Stloc_2)
            );
        matcher.Advance(1);//Could likely just use `useEnd = true` above but oh well
        matcher.Insert(Transpilers.EmitDelegate(InsertStripped));

        return matcher.InstructionEnumeration();
    }
    public static PlatformServices InsertStripped(PlatformServices original)
    {
        Plugin.Logger.LogMessage(useStrippedServices ? "Inserting stripped platform services" : "No stripped platform services required");
        return useStrippedServices ? new StrippedPlatformServiesSteam() : original;
    }
}
