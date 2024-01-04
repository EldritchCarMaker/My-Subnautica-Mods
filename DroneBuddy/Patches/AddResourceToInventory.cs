using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace DroneBuddy.Patches;

[HarmonyPatch(typeof(BreakableResource))]
internal class AddResourceToInventory
{
    internal static Dictionary<Vector3, IItemsContainer> resourcesToInventories = new Dictionary<Vector3, IItemsContainer>();
    [HarmonyPatch(nameof(BreakableResource.SpawnResourceFromPrefab))]
    [HarmonyPatch(MethodType.Enumerator, typeof(AssetReferenceGameObject), typeof(Vector3), typeof(Vector3))]
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var matcher = new CodeMatcher(instructions);

        matcher.MatchForward(false, new CodeMatch(inst => inst.opcode == OpCodes.Ldfld && inst.operand is FieldInfo info && info.Name == "position"));
        var fieldInfo = matcher.Instruction.operand as FieldInfo;//I am not getting the field info through reflection myself. Just get the operand from earlier in the method

        matcher.MatchForward(true, new CodeMatch(OpCodes.Ldloc_1));
        matcher.MatchForward(true, new CodeMatch(OpCodes.Ldloc_1));
        matcher.Advance(1);

        matcher.InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_1));
        matcher.InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_0));
        matcher.InsertAndAdvance(new CodeInstruction(OpCodes.Ldfld, fieldInfo));
        matcher.Insert(Transpilers.EmitDelegate(AddToInventory));

        return matcher.InstructionEnumeration();
    }
    public static void AddToInventory(GameObject obj, Vector3 pos)
    {
        if (!resourcesToInventories.TryGetValue(pos, out var inventory))
            return;

        var pick = obj.GetComponent<Pickupable>();
        if (!pick)
        {
            Plugin.Logger.LogError($"Pickuppable null on gameobject from outcrop {obj}");
        }
        pick.Pickup(true);
        inventory.AddItem(new InventoryItem(pick));

        resourcesToInventories.Remove(obj.transform.position);
    }
}
