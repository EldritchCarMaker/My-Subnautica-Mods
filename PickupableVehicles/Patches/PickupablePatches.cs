using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static HandReticle;

namespace PickupableVehicles.Patches
{
    [HarmonyPatch(typeof(Pickupable))]
    internal class PickupablePatches
    {
        [HarmonyPatch(nameof(Pickupable.Drop), 
            new[] {typeof(Vector3), typeof(Vector3), typeof(bool)})]
        public static void Prefix(Pickupable __instance, ref Vector3 dropPosition)
        {
            TechType type = __instance.GetTechType();
            if (type == TechType.Seamoth || type == TechType.Exosuit)
            {
                __instance.randomizeRotationWhenDropped = false;
                dropPosition += 6f * MainCamera.camera.transform.forward;
            }
#if SN
            else if(type == TechType.Cyclops)
            {
                dropPosition += 15f * MainCamera.camera.transform.forward;
            }
            else if(type == TechType.RocketBase)
            {
                dropPosition += 35f * MainCamera.camera.transform.forward;
                __instance.randomizeRotationWhenDropped = false;
            }
#endif
            else if (type.ToString().ToLower().Contains("seatruck"))
            {
                dropPosition += 6f * MainCamera.camera.transform.forward;
            }
        }
        [HarmonyPatch(nameof(Pickupable.Drop),
            new[] { typeof(Vector3), typeof(Vector3), typeof(bool) })]
        public static void Postfix(Pickupable __instance)
        {
            TechType type = __instance.GetTechType();
            if(type == TechType.Exosuit)
            {
                var exo = __instance.GetComponent<Exosuit>();
                if (!exo) return;
                exo.timeOnGround = 0f;
                exo.onGround = false;
            }
            else if (type == TechType.Seamoth || type == TechType.Exosuit
#if SN
                || type == TechType.Cyclops || type == TechType.RocketBase) 
#else
                || type.ToString().ToLower().Contains("seatruck"))
#endif
            {
                __instance.gameObject.transform.eulerAngles = new Vector3(0, 0, 0);
            }
        }
    }
}
