using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
#if !SN2
using Logger = QModManager.Utility.Logger;
#endif

namespace AutoStorageTransferCompatibility.Patches
{
    public static class ReflectionHelp
    {
        public static void PatchIfExists(Harmony harmony, string assemblyName, string typeName, string methodName, HarmonyMethod prefix = null, HarmonyMethod postfix = null, HarmonyMethod transpiler = null)
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
