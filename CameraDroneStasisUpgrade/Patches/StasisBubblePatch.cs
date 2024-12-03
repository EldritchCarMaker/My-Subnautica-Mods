using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;

namespace CameraDroneStasisUpgrade.Patches
{
    [HarmonyPatch(typeof(StasisSphere))]
    internal class StasisBubblePatch
    {
        [HarmonyPatch(nameof(StasisSphere.Freeze))]
        public static bool Prefix(Collider other)
        {
            if (other == null || other.gameObject == null || other.gameObject.GetComponent<MapRoomCamera>() == null) return true;
            
            var man = other.GetComponent<CameraDroneUpgrades.maproomdroneupgrades>();
            if (man == null) return true;

            if(man.equippedUpgrades.Contains(QMod.moduleTechType))
                return false;
            return true;
        }
    }
}
