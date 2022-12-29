using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FasterSnowFoxCharging;
using HarmonyLib;
using UnityEngine;
using Logger = QModManager.Utility.Logger;

namespace QuantumLockerMute
{
    internal class QuantumLockerMute
    {
#if BZ
        [HarmonyPatch(typeof(QuantumLocker))]
        [HarmonyPatch("ShowThrusterVFX")]
        internal class PatchQuantumLockerVFX

        {
            [HarmonyPostfix]

            public static void Postfix(QuantumLocker __instance)
            {
                __instance.hoverLoopingSound.Stop();
            }
        }
#endif
    }
}