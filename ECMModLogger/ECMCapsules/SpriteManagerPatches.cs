using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using SMLHelper.V2.Utility;
using System.IO;
using System.Reflection;

namespace ECMModLogger.ECMCapsules
{
    [HarmonyPatch(typeof(SpriteManager))]
    public class SpriteManagerPatches
    {
        private static Atlas.Sprite EldSprite = ImageUtils.LoadSpriteFromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "eldritch_logo.png"));
        [HarmonyPatch(nameof(SpriteManager.Get))]
        [HarmonyPatch(new[] {typeof(TechType)})]
        public static bool Prefix(ref Atlas.Sprite __result)
        {
            if (!CapsuleManager.CollectedFive) return true;

            __result = EldSprite;
            return false;
        }
        public static void RefreshSprite()//for testing different sprites
        {
            EldSprite = ImageUtils.LoadSpriteFromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "spin-record.gif"));
        }
    }
}
