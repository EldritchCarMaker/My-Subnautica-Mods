using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

namespace Snomod.Patches
{
    [HarmonyPatch(typeof(LavaMeteor))]
    internal class LavaMeteorPatches
    {
        [HarmonyPatch(nameof(LavaMeteor.Detonate))]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var cm = new CodeMatcher(instructions);
            cm.Start();
            cm.SearchForward((instruction) =>//look for the GetComponent call, it's only used once in the method and is the only generic there
                instruction.opcode == OpCodes.Callvirt &&
                ((MethodInfo)instruction.operand).IsGenericMethod);

            cm.InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_3));//load the collider
            cm.InsertAndAdvance(new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(Collider), "get_attachedRigidbody")));//call the getter for attached rigidbody from the collider, and load the rigidbody
            cm.InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_0));//load "this" argument, aka the instance
            cm.InsertAndAdvance(new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(LavaMeteorPatches), nameof(LavaMeteorPatches.ExplodeEffects))));//call method using the two above values

            return cm.InstructionEnumeration();
        }

        public const float ExplosionForce = 5000;

        public static void ExplodeEffects(Rigidbody rigidbody, LavaMeteor instance)
        {
            rigidbody.AddExplosionForce(ExplosionForce, instance.transform.position, instance.explodeRadius);

            if (rigidbody.TryGetComponent(out BreakableResource resource)) resource.BreakIntoResources();
            
            if (rigidbody.TryGetComponent(out EscapePod escapePod)) escapePod.liveMixin.data.destroyOnDeath = true;
        }
    }
}
