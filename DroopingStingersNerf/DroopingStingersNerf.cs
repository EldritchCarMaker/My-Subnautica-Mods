using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using System;
using UnityEngine;

namespace DroopingStingersNerf
{
    [HarmonyPatch(typeof(HangingStinger), "Update")]
    public class DroopingStingers
    {
        [HarmonyPostfix]
        private static void startPostFix(HangingStinger __instance)
        {
            if(__instance._venomAmount > 1f)
            {
                __instance._venomAmount = 1f;
            }
        }
    }
    [HarmonyPatch(typeof(HangingStinger), nameof(HangingStinger.OnCollisionEnter))]
    public class DroopingStingersTooGood
    {
        [HarmonyPrefix]
        private static bool StartPrefix(HangingStinger __instance, Collision other)
        {
            
            if (__instance._venomAmount >= 1f && other.gameObject.GetComponentInChildren<LiveMixin>() != null && other.gameObject.GetComponent(typeof(DamageOverTime)) == null)
	        {
		        DamageOverTime damageOverTime = other.gameObject.AddComponent<DamageOverTime>();
	        	damageOverTime.totalDamage = 15f;
	        	damageOverTime.duration = 2.5f * (float)__instance.size;
	        	damageOverTime.damageType = DamageType.Poison;
	        	damageOverTime.ActivateInterval(0.5f);
	        	__instance._venomAmount = 0f;
	        	__instance.venomRechargeTime = UnityEngine.Random.value * 5f + 5f;
	        }

            return false;
        }
    }
}
