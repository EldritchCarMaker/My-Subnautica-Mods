using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
namespace CyclopsTorpedoes.Patches
{
    [HarmonyPatch(typeof(CyclopsDecoyLoadingTube))]
    internal class CyclopsDecoyLoadingTubePatches
    {
        [HarmonyTranspiler]
        [HarmonyPatch(nameof(CyclopsDecoyLoadingTube.RecalcDecoyTotals))]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> codeInstructions = new List<CodeInstruction>(instructions);

            int count = instructions.Count();
            for (int i = 0; i < count; i++)
            {
                //Current Instruction
                CodeInstruction currentInstruction = codeInstructions[i];
                //Next Instruction or null if current is last instruction
                CodeInstruction secondInstruction = i + 1 < count - 1 ? codeInstructions[i + 1] : null;

                if (
                    // Ensure current opcode matches what you want to change
                    currentInstruction.opcode == OpCodes.Callvirt &&
                    // Ensure Next opcode is not null
                    secondInstruction != null && 
                    secondInstruction.opcode != null &&
                    // Ensure Next opcode matches the code after the one you want to change
                    secondInstruction.opcode == OpCodes.Brfalse
                    )
                {
                    //do something here to change things at currentInstruction location.
                    codeInstructions.Insert(i + 1, Transpilers.EmitDelegate(CheckForTorpedoTechType));
                    break;
                }
            } 
            return codeInstructions.AsEnumerable();
        }



        [HarmonyTranspiler]
        [HarmonyPatch(nameof(CyclopsDecoyLoadingTube.TryRemoveDecoyFromTube))]
        public static IEnumerable<CodeInstruction> ConsumeDecoyTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> codeInstructions = new List<CodeInstruction>(instructions);

            int count = instructions.Count();
            for (int i = 0; i < count; i++)
            {
                //Current Instruction
                CodeInstruction currentInstruction = codeInstructions[i];
                //Next Instruction or null if current is last instruction
                CodeInstruction secondInstruction = i + 1 < count - 1 ? codeInstructions[i + 1] : null;

                if (
                    // Ensure current opcode matches what you want to change
                    currentInstruction.opcode == OpCodes.Callvirt &&
                    // Ensure Next opcode is not null
                    secondInstruction != null &&
                    secondInstruction.opcode != null &&
                    // Ensure Next opcode matches the code after the one you want to change
                    secondInstruction.opcode == OpCodes.Brfalse
                    )
                {
                    //do something here to change things at currentInstruction location.
                    codeInstructions.Insert(i + 1, Transpilers.EmitDelegate(CheckForTorpedoTechType));
                    break;
                }
            }
            return codeInstructions.AsEnumerable();
        }

        //Return false if None or a torpedo
        public static bool CheckForTorpedoTechType(TechType techType)
        {
            if(techType == TechType.None) return false;
            if(CyclopsExternalCamsPatches.torpedoTypes.Any(type => type.techType == techType)) return false;

            return true;
        }
    }
}
