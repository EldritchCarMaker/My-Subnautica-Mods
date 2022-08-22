using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
         */
        
    }
}
