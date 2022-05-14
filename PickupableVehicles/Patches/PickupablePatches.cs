using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

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
                dropPosition += 6f * MainCamera.camera.transform.forward;
            }
        }
        [HarmonyPatch(nameof(Pickupable.Drop),
            new[] { typeof(Vector3), typeof(Vector3), typeof(bool) })]
        public static void Postfix(Pickupable __instance)
        {
            TechType type = __instance.GetTechType();
            if (type == TechType.Seamoth || type == TechType.Exosuit)
            {
                __instance.gameObject.transform.eulerAngles = new Vector3(0, 0, 0);
            }
        }
    }
}
