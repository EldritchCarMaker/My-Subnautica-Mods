using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CyclopsVehicleUpgradeConsole
{
    public class Helpers
    {
        public static void PatchIfExists(Harmony harmony, string assemblyName, string typeName, string methodName, HarmonyMethod prefix, HarmonyMethod postfix, HarmonyMethod transpiler)
        {
            var assembly = FindAssembly(assemblyName);
            if (assembly == null)
            {
                return;
            }

            Type targetType = assembly.GetType(typeName);
            if (targetType == null)
            {
                return;
            }

            var targetMethod = FindMethod(methodName, targetType);
            if (targetMethod != null)
            {
                harmony.Patch(targetMethod, prefix, postfix, transpiler);
            }
        }

        private static Assembly FindAssembly(string assemblyName)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                if (assembly.FullName.StartsWith(assemblyName + ","))
                    return assembly;

            return null;
        }

        public static MethodInfo FindMethod(string assemblyName, string typeName, string methodName)
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

            var targetMethod = AccessTools.Method(targetType, methodName);
            return targetMethod;
        }

        public static MethodInfo FindMethod(string methodName, Type type)
        {
            return AccessTools.Method(type, methodName);
        }
    }
}