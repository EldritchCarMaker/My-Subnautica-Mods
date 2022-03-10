using System;
using System.Collections.Generic;
using HarmonyLib;

namespace RALIV.Subnautica.AquariumBreeding
{
    // Token: 0x02000004 RID: 4
    [HarmonyPatch(typeof(Aquarium))]
    [HarmonyPatch("AddItem")]
    public class Aquarium_AddItem_Patch
    {
        // Token: 0x06000006 RID: 6 RVA: 0x000021F0 File Offset: 0x000003F0
        [HarmonyPostfix]
        public static void Postfix(Aquarium __instance)
        {
            AquariumInfo.Update(__instance);
        }

        // Token: 0x04000007 RID: 7
        public static Dictionary<Aquarium, AquariumInfo> aquariumInfo = new Dictionary<Aquarium, AquariumInfo>();
    }
}
