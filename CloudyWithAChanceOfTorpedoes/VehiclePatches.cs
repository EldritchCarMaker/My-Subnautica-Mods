using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

namespace CloudyWithAChanceOfTorpedoes;

[HarmonyPatch(typeof(Vehicle), nameof(Vehicle.TorpedoShot))]
internal class VehiclePatches
{
    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var matcher = new CodeMatcher(instructions);
        matcher.MatchForward(false, new CodeMatch((instruction) => instruction.opcode == OpCodes.Callvirt && instruction.operand is MethodInfo info && info.Name == nameof(ItemsContainer.DestroyItem)));

        matcher.InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_2));
        matcher.SetAndAdvance(OpCodes.Call, AccessTools.Method(typeof(VehiclePatches), nameof(VehiclePatches.CheckForModule)));

        return matcher.InstructionEnumeration();
    }

    public static bool CheckForModule(ItemsContainer container, TechType torpType, Transform muzzle)
    {
        if (container == null) return false;

        if (muzzle == null) return container.DestroyItem(torpType);
        var vehcle = muzzle.GetComponentInParent<Vehicle>();
        if (!vehcle) return container.DestroyItem(torpType);

        if(vehcle.modules.GetCount(ChanceOfTorpedoes.ModuleType) <= 0) return container.DestroyItem(torpType);

        if (vehcle.ConsumeEnergy(TechType.SeamothSonarModule)) return true;
        return false;
    }
}
