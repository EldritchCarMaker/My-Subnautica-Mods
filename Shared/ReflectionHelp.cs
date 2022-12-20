using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Logger = QModManager.Utility.Logger;

namespace Shared
{
    public class ReflectionHelp
    {
        public static Assembly FindAssembly(string assemblyName)
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
        public static FieldInfo FindField(string assemblyName, string typeName, string fieldName)
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

            var targetField = AccessTools.Field(targetType, fieldName);
            if (targetField == null)
            {
                Logger.Log(Logger.Level.Debug, "Could not find field " + typeName + "." + fieldName + ", the mod/assembly " + assemblyName + " might have been changed");
            }
            return targetField;
        }
    }
}
