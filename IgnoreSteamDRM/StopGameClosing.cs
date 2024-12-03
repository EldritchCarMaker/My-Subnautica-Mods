using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace IgnoreSteamDRM;

[HarmonyPatch(typeof(PlatformServicesSteam), nameof(PlatformServicesSteam.InitializeAsync), MethodType.Enumerator)]
internal class StopGameClosing
{
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var matcher = new CodeMatcher(instructions);

        matcher.MatchForward(false, new CodeMatch(inst => inst.operand is MethodInfo info && info.Name == nameof(PlatformServicesSteam.RestartInSteam)));//Find restart line
        matcher.Advance(-1);//Go back by one to catch the `this` argument loading
        matcher.SetOpcodeAndAdvance(OpCodes.Nop);//The instruction has a label, we *need* to keep that label in place, but we cant have the instruction actually fucking with the stack anymore
        matcher.RemoveInstruction();//Remove the RestartInSteam call

        matcher.MatchForward(false, new CodeMatch(inst => (inst.operand is string str) && str.Contains("initialize")));//Find initialize steamworks string
        matcher.Advance(2);//Advance past the debug log, and onto the application quit
        matcher.RemoveInstruction();//Remove the application quit call
        matcher.Insert(Transpilers.EmitDelegate(ForceSteamServices));
        //Method now returns

        return matcher.InstructionEnumeration();
    }
    public static void ForceSteamServices()
    {
        Plugin.Logger.LogWarning("Forcing game to continue without quitting");
        Plugin.Logger.LogWarning("That's the main point of this mod but it's still not recommended, it's unsure what side effects this could cause");

        //Tell the other patcher to insert our stripped version
        InsertStrippedPlatformServices.useStrippedServices = true;
    }
}
