using System;
using System.Collections.Generic;
using HarmonyLib;

namespace RALIV.Subnautica.AquariumBreeding
{
    // Token: 0x02000005 RID: 5
    [HarmonyPatch(typeof(Aquarium))]
    [HarmonyPatch("RemoveItem")]
    public class Aquarium_RemoveItem_Patch
    {
        // Token: 0x06000009 RID: 9 RVA: 0x000021F0 File Offset: 0x000003F0
        [HarmonyPostfix]
        public static void Postfix(Aquarium __instance)
        {
            AquariumInfo.Update(__instance);
        }

        // Token: 0x04000008 RID: 8
        public static Dictionary<Aquarium, AquariumInfo> aquariumInfo = new Dictionary<Aquarium, AquariumInfo>();
    }
}
