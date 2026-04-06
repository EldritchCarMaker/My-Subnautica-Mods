using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Snomod.Prefabs;
using Snomod.MonoBehaviours;
using System.IO;
using UnityEngine;

namespace Snomod.Patches
{
    [HarmonyPatch(typeof(SpriteManager))]
    internal class SpriteManagerPatches
    {
        [HarmonyPatch(nameof(SpriteManager.Get), new[] {typeof(TechType)})]
        public static bool Prefix(TechType techType, ref Sprite __result)
        {
            if (techType != Amogus.TT) return true;
            if (GUIItemsContainerPatches.LastColorType == MogusColorChanger.ColorType.None) return true;

            __result = Amogus.bundle.LoadAsset<UnityEngine.Sprite>($"amogusIcon{GUIItemsContainerPatches.LastColorType}");
            return false;
        }
    }
}
