using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using QModManager.Utility;

namespace AntiUselessAirBladders.Patches
{
    [HarmonyPatch(typeof(AirBladder))]
    internal class AirBladderPatches
    {
        [HarmonyPatch(nameof(AirBladder.UpdateInflateState))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions);
            matcher.SearchForward((instruction) => CheckInstruction(instruction));
            matcher.RemoveInstructionsInRange(matcher.Pos - 2, matcher.Pos + 1);
            matcher.SearchForward((instruction) => instruction.opcode == OpCodes.Ldloc_3);
            matcher.Opcode = OpCodes.Ldloc_2;
            return matcher.InstructionEnumeration();
        }

        public static bool CheckInstruction(CodeInstruction instruction)
        {
            Console.WriteLine($"opcode {instruction.opcode}, {instruction.operand}");
            bool result = instruction.opcode == OpCodes.Callvirt && 
                ((MethodInfo)instruction.operand) == AccessTools.Method(typeof(OxygenManager), nameof(OxygenManager.RemoveOxygen));
            if (result) Console.WriteLine("Found!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            return result;
        }
    }
}
