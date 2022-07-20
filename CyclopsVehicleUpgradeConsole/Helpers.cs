using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Logger = QModManager.Utility.Logger;

namespace CyclopsVehicleUpgradeConsole
{
    public class Helpers
    {
        public static void PatchIfExists(Harmony harmony, string assemblyName, string typeName, string methodName, HarmonyMethod prefix, HarmonyMethod postfix, HarmonyMethod transpiler)
        {
            var assembly = FindAssembly(assemblyName);
            if (assembly == null)
            {
                Logger.Log(Logger.Level.Info, "Could not find assembly " + assemblyName + ", don't worry this probably just means you don't have the mod installed");
                return;
            }

            Type targetType = assembly.GetType(typeName);
            if (targetType == null)
            {
                Logger.Log(Logger.Level.Info, "Could not find class/type " + typeName + ", the mod/assembly " + assemblyName + " might have changed");
                return;
            }

            Logger.Log(Logger.Level.Info, "Found targetClass " + typeName);
            var targetMethod = FindMethod(methodName, targetType);
            if (targetMethod != null)
            {
                Logger.Log(Logger.Level.Info, "Found targetMethod " + typeName + "." + methodName + ", Patching...");
                harmony.Patch(targetMethod, prefix, postfix, transpiler);
                Logger.Log(Logger.Level.Info, "Patched " + typeName + "." + methodName);
            }
            else
            {
                Logger.Log(Logger.Level.Info, "Could not find method " + typeName + "." + methodName + ", the mod/assembly " + assemblyName + " might have been changed");
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
                Logger.Log(Logger.Level.Debug, "Could not find assembly " + assemblyName + ", don't worry this probably just means you don't have the mod installed");
                return null;
            }

            Type targetType = assembly.GetType(typeName);
            if (targetType == null)
            {
                Logger.Log(Logger.Level.Debug, "Could not find class/type " + typeName + ", the mod/assembly " + assemblyName + " might have changed");
                return null;
            }

            var targetMethod = AccessTools.Method(targetType, methodName);
            if (targetMethod == null)
            {
                Logger.Log(Logger.Level.Debug, "Could not find method " + typeName + "." + methodName + ", the mod/assembly " + assemblyName + " might have been changed");
            }
            return targetMethod;
        }

        public static MethodInfo FindMethod(string methodName, Type type)
        {
            return AccessTools.Method(type, methodName);
        }
    }
}
