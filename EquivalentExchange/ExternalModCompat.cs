using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static EquivalentExchange.QMod;
#if !SN2
using Logger = QModManager.Utility.Logger;
#endif

#if SN1
using FCS_AlterraHub.Systems;
#endif

namespace EquivalentExchange
{
    public class ExternalModCompat
    {
        public static void SetTechTypeValue(TechType tech, int value)//to use, look at comment block below method
        {
            if(!QMod.config.ModifiedItemCosts.TryGetValue(tech, out _)) 
            {
                QMod.config.ModifiedItemCosts.Add(tech, value);
            }
            else
            {
                QMod.config.ModifiedItemCosts[tech] = value;
            }
        }
        /*use something similar to this in your mod in order to use this method without requiring a dependency/reference

            if (QModManager.API.QModServices.Main.ModPresent("EquivalentExchange"))
            {
                AlterExchangeValue(MyItemTechType, MyExchangeValueFloat);
            }
          
            public static void AlterExchangeValue(TechType techType, float value) 
            {
                MethodInfo SetValueMethod = FindMethod("EquivalentExchange", "EquivalentExchange.ExternalModCompat", "SetTechTypeValue");
                if(SetValueMethod == null) return;//maybe put a log here too, up to you
                SetValueMethod.Invoke(null, new object[] { techType, value });
            }
          
         */
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
                LogDebug("Could not find assembly " + assemblyName + ", don't worry this probably just means you don't have the mod installed");
                return null;
            }

            Type targetType = assembly.GetType(typeName);
            if (targetType == null)
            {
                LogDebug("Could not find class/type " + typeName + ", the mod/assembly " + assemblyName + " might have changed");
                return null;
            }

            var targetMethod = AccessTools.Method(targetType, methodName);
            if (targetMethod == null)
            {
                LogDebug("Could not find method " + typeName + "." + methodName + ", the mod/assembly " + assemblyName + " might have been changed");
            }
            return targetMethod;
        }
        public static FieldInfo FindField(string assemblyName, string typeName, string fieldName)
        {
            var assembly = FindAssembly(assemblyName);
            if (assembly == null)
            {
                LogDebug("Could not find assembly " + assemblyName + ", don't worry this probably just means you don't have the mod installed");
                return null;
            }

            Type targetType = assembly.GetType(typeName);
            if (targetType == null)
            {
                LogDebug("Could not find class/type " + typeName + ", the mod/assembly " + assemblyName + " might have changed");
                return null;
            }

            var targetField = AccessTools.Field(targetType, fieldName);
            if (targetField == null)
            {
                LogDebug("Could not find field " + typeName + "." + fieldName + ", the mod/assembly " + assemblyName + " might have been changed");
            }
            return targetField;
        }
#if SN1
        public static bool AddFCSCredit(decimal amount)
        {
            CardSystem.main.AddFinances(amount);
            return true;
        }
        public static bool RemoveFCSCredit(decimal amount)
        {
            CardSystem.main.RemoveFinances(amount);
            return true;
        }
        public static decimal GetFCSCredit() => CardSystem.main.GetAccountBalance();
        public static GameObject GetFCSPDA() => FCS_AlterraHub.Mods.FCSPDA.Mono.FCSPDAController.Main?._screen;
#endif
    }
}
