using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Logger = QModManager.Utility.Logger;

namespace EquivalentExchange
{
    public class ExternalModCompat
    {
        public static void SetTechTypeValue(TechType tech, int value)//to use, look at comment block below method
        {
            if(!QMod.config.BaseMaterialCosts.TryGetValue(tech, out _)) 
            {
                QMod.config.BaseMaterialCosts.Add(tech, value);
            }
            else
            {
                QMod.config.BaseMaterialCosts[tech] = value;
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

        public static bool AddFCSCredit(decimal amount)
        {
            FieldInfo mainCardSystem = FindField("FCS_AlterraHub", "FCS_AlterraHub.Systems.CardSystem", "main");
            if(mainCardSystem == null)
            {
                return false;
            }

            MethodInfo addFinancesMethod = FindMethod("FCS_AlterraHub", "FCS_AlterraHub.Systems.CardSystem", "AddFinances");
            if(addFinancesMethod == null)
            {
                return false;
            }

            addFinancesMethod.Invoke(mainCardSystem.GetValue(null), new object[] { amount });
            return true;
        }
        public static bool RemoveFCSCredit(decimal amount)
        {
            FieldInfo mainCardSystem = FindField("FCS_AlterraHub", "FCS_AlterraHub.Systems.CardSystem", "main");
            if (mainCardSystem == null)
            {
                return false;
            }

            MethodInfo addFinancesMethod = FindMethod("FCS_AlterraHub", "FCS_AlterraHub.Systems.CardSystem", "RemoveFinances");
            if (addFinancesMethod == null)
            {
                return false;
            }

            addFinancesMethod.Invoke(mainCardSystem.GetValue(null), new object[] { amount });
            return true;
        }
        public static decimal GetFCSCredit()
        {
            FieldInfo mainCardSystem = FindField("FCS_AlterraHub", "FCS_AlterraHub.Systems.CardSystem", "main");
            if (mainCardSystem == null)
            {
                return 0;
            }

            MethodInfo addFinancesMethod = FindMethod("FCS_AlterraHub", "FCS_AlterraHub.Systems.CardSystem", "GetAccountBalance");
            if (addFinancesMethod == null)
            {
                return 0;
            }

            return (decimal)addFinancesMethod.Invoke(mainCardSystem.GetValue(null), null);
        }
        public static object SetFCSConvertIcons()
        {
            FieldInfo mainPDAField = FindField("FCS_AlterraHub", "FCS_AlterraHub.Mods.FCSPDA.Mono.FCSPDAController", "Main");
            if (mainPDAField == null)
            {
                return null;
            }

            FieldInfo mainPDAScreenField = FindField("FCS_AlterraHub", "FCS_AlterraHub.Mods.FCSPDA.Mono.FCSPDAController", "_screen");
            if (mainPDAScreenField == null)
            {
                return null;
            }
            return mainPDAScreenField.GetValue(mainPDAField.GetValue(null));
        }
    }
}
