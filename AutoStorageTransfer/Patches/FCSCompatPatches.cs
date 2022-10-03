using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AutoStorageTransfer.Monobehaviours;
using HarmonyLib;
using UnityEngine;
using Logger = QModManager.Utility.Logger;

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

            Logger.Log(Logger.Level.Debug, "Found targetClass " + typeName);
            var targetMethod = AccessTools.Method(targetType, methodName);
            if (targetMethod != null)
            {
                Logger.Log(Logger.Level.Debug, "Found targetMethod " + typeName + "." + methodName + ", Patching...");
                harmony.Patch(targetMethod, prefix, postfix, transpiler);
                Logger.Log(Logger.Level.Debug, "Patched " + typeName + "." + methodName);
            }
            else
            {
                Logger.Log(Logger.Level.Debug, "Could not find method " + typeName + "." + methodName + ", the mod/assembly " + assemblyName + " might have been changed");
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
                Logger.Log(Logger.Level.Debug, "Could not find assembly " + assemblyName + ", don't worry this probably just means you don't have the mod installed");
                return null;
            }

            Type targetType = assembly.GetType(typeName);
            if (targetType == null)
            {
                Logger.Log(Logger.Level.Debug, "Could not find class/type " + typeName + ", the mod/assembly " + assemblyName + " might have changed");
                return null;
            }
            return targetType;
        }
    }
}
