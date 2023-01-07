using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AutoStorageTransfer.Monobehaviours;
using HarmonyLib;
using UnityEngine;
#if !SN2
using Logger = QModManager.Utility.Logger;
#endif

namespace AutoStorageTransfer.Patches
{
    internal class FCSCompatPatches
    {
        public static void PatchFCS(Harmony harmony)
        {
            var patchMethod = AccessTools.Method(typeof(FCSCompatPatches), nameof(Postfix));

            PatchIfExists(harmony, "FCS_StorageSolutions", "FCS_StorageSolutions.Mods.DataStorageSolutions.Mono.Terminal.DSSTerminalDisplayManager", "Setup", null, new HarmonyMethod(patchMethod), null);
        }
        public static void Postfix(MonoBehaviour __instance)
        {
            __instance.gameObject.AddComponent<FCSCompatStorageTransfer>();
        }
        public static void PatchIfExists(Harmony harmony, string assemblyName, string typeName, string methodName, HarmonyMethod prefix, HarmonyMethod postfix, HarmonyMethod transpiler)
        {
            var targetType = FindType(assemblyName, typeName);

            var targetMethod = AccessTools.Method(targetType, methodName);
            if (targetMethod != null)
            {
                harmony.Patch(targetMethod, prefix, postfix, transpiler);
            }
        }
        public static Assembly FindAssembly(string assemblyName)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                if (assembly.FullName.StartsWith(assemblyName + ","))
                    return assembly;

            return null;
        }
        public static Type FindType(string assemblyName, string typeName)
        {
            var assembly = FindAssembly(assemblyName);
            if (assembly == null)
            {
                return null;
            }

            Type targetType = assembly.GetType(typeName);
            if (targetType == null)
            {
                return null;
            }
            return targetType;
        }
    }
}
