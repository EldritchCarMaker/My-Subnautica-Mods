using System;
using System.IO;
using System.Reflection;
using HarmonyLib;
using QModManager.API.ModLoading;
using QModManager.Utility;

namespace RALIV.Subnautica.AquariumBreeding
{
    // Token: 0x02000007 RID: 7
    [QModCore]
    public class QMod
    {
        // Token: 0x0600000F RID: 15 RVA: 0x0000231A File Offset: 0x0000051A
        public static void Log(string message)
        {
            File.AppendAllText(QMod.logFile, string.Concat(new object[]
            {
                DateTime.Now,
                "\r\n",
                message,
                "\r\n"
            }));
        }

        // Token: 0x06000010 RID: 16 RVA: 0x00002352 File Offset: 0x00000552
        [QModPatch]
        public static void Patch()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var CyclopsLockers = ($"Nagorogan_{assembly.GetName().Name}");
            Logger.Log(Logger.Level.Info, $"Patching {CyclopsLockers}");
            Harmony harmony = new Harmony(CyclopsLockers);
            harmony.PatchAll(assembly);
            Logger.Log(Logger.Level.Info, "Patched successfully!");
        }

        // Token: 0x04000009 RID: 9
        private static string logFile = Path.GetFullPath(".\\QMods\\AquariumBreeding\\AquariumBreeding.log");
    }
}
