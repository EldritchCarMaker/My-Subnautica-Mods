using FCS_AlterraHub.Systems;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Logger = QModManager.Utility.Logger;

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
    }
}
