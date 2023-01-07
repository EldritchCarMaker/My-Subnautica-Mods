using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using AntiUselessAirBladders.Monobehaviours;
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
            matcher.Start();
            matcher.SearchForward((instruction) => instruction.opcode == OpCodes.Callvirt &&
                ((MethodInfo)instruction.operand) == AccessTools.Method(typeof(OxygenManager), nameof(OxygenManager.RemoveOxygen)));
            matcher.Advance(-2);

            matcher.SetAndAdvance(OpCodes.Ldarg_0, null);
            matcher.SetAndAdvance(OpCodes.Ldloc_2, null);
            matcher.Set(OpCodes.Callvirt, AccessTools.Method(typeof(AirBladderOxygen), nameof(AirBladderOxygen.ConsumeOxygen)));

            return matcher.InstructionEnumeration();
        }
        [HarmonyPatch(nameof(AirBladder.Start))]
        public static void Postfix(AirBladder __instance)
        {
            __instance.gameObject.AddComponent<AirBladderOxygen>();
        }
    }
}
