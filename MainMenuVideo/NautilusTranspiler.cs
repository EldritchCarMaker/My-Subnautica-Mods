using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace MainMenuVideo;

//Nautilus patchers are internal, so can't use attribute patching for this one
internal class NautilusTranspiler
{
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var matcher = new CodeMatcher(instructions);

        matcher.MatchForward(true, new CodeMatch((inst) => inst.opcode == OpCodes.Call && inst.operand is MethodInfo method && method.Name == "RefreshModOptions"));
        matcher.Advance(1);
        var firstIndex = matcher.Pos;

        matcher.MatchForward(true, new CodeMatch((inst) => inst.opcode == OpCodes.Call && inst.operand is MethodInfo method && method.Name == "OnActiveModChanged"));
        matcher.Advance(-4);//Want to keep a handful of the calls (to avoid reflection later)
        var secondIndex = matcher.Pos;

        matcher.RemoveInstructionsInRange(firstIndex, secondIndex);
        matcher.MatchBack(false, new CodeMatch((inst) => inst.opcode == OpCodes.Ldsfld && inst.operand is FieldInfo field && field.Name == "_choiceOption"));
        matcher.Advance(1);
        matcher.SetOpcodeAndAdvance(OpCodes.Dup);
        matcher.InsertAndAdvance(Transpilers.EmitDelegate(SelectRandom));

        return matcher.InstructionEnumeration();
    }
    public static int SelectRandom(uGUI_Choice choiceMenu)
    {
        if (Plugin.foundFileNames.Count == 0) return 0;
        var randomIndex = UnityEngine.Random.Range(0, Plugin.foundFileNames.Count);
        var randomFileName = Plugin.foundFileNames[randomIndex];

        return choiceMenu.options.IndexOf(randomFileName);
    }
}
