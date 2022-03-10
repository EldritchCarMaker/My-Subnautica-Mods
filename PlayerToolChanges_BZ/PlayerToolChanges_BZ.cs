using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;
using Logger = QModManager.Utility.Logger;

namespace RepairToolChanges_SN
{
    internal class RepairToolChanges
    {
        [HarmonyPatch(typeof(PlayerTool))]
        [HarmonyPatch("OnDraw")]
        internal class PatchPlayerToolAwake

        {
            [HarmonyPostfix]
            
            public static void Postfix(PlayerTool __instance)
            {
                if (__instance.GetType() == typeof(Welder))
                {
                    Welder Welder = __instance as Welder;

                    float repairRate = QMod.config.repairToolRepairRate;
                    float energyCost = QMod.config.repairToolEnergyCost;
                    //float repairRate = 2;
                    //float energyCost = 2;

                    Welder.healthPerWeld = repairRate * 10f;
                    Welder.weldEnergyCost = energyCost;
                }
                else if(__instance.GetType() == typeof(Knife))
                {
                    Knife knife = __instance as Knife;

                    float damage = QMod.config.knifeDamage;
                    float reach = QMod.config.knifeRange;

                    knife.damage = damage * 25f;
                    knife.attackDist = reach * 2f;
                }
                else if(__instance.GetType() == typeof(HeatBlade))
                {
                    HeatBlade heatblade = __instance as HeatBlade;

                    heatblade.attackDist = QMod.config.knifeRange * 2f;
                    heatblade.damage = QMod.config.knifeDamage * 25f;
                }
                else if (__instance.GetType() == typeof(LaserCutter))
                {
                    LaserCutter laserCutter = __instance as LaserCutter;

                    float cutRate = QMod.config.laserCutterCutRate;
                    float energyCost = QMod.config.laserCutterEnergyCost;

                    laserCutter.healthPerWeld = cutRate * 25f;
                    laserCutter.laserEnergyCost = energyCost;
                    /*}else if(__instance.GetType() == typeof(PropulsionCannonWeapon))
                    {
                        PropulsionCannonWeapon propCannon = __instance as PropulsionCannonWeapon;

                        float pickupDist = QMod.config.propCannonPickupDist;
                        float shootingForce = QMod.config.propCannonShootForce;
                        //float EnergyCost = QMod.config.propCannonEnergyCost;
                        //Can't modify the energy cost variables due to being constants
                        propCannon.pickupDistance = pickupDist * 18;
                        propCannon.shootForce = shootingForce * 50;
                        //propCannon.
                    */
                }
            }
        }
        [HarmonyPatch(typeof(PropulsionCannon))]
        [HarmonyPatch(nameof(PropulsionCannon.Update))]
        internal class patchPropCannon
        {
            [HarmonyPrefix]
            public static void PropCannonPreFix(PropulsionCannon __instance)
            {
                if (__instance.grabbedObject != null)
                {
                    if (__instance.grabbedObject.GetComponent<Rigidbody>() != null)
                    {
                        for (int i = 0; i < __instance.elecLines.Length; i++)
                        {
                            VFXElectricLine vfxelectricLine = __instance.elecLines[i];
                            vfxelectricLine.origin = __instance.muzzle.position;
                            vfxelectricLine.target = __instance.grabbedObjectCenter;
                            vfxelectricLine.originVector = __instance.muzzle.forward;
                        }
                    }
                    __instance.energyInterface.ConsumeEnergy(Time.deltaTime * 0.7f * QMod.config.propCannonEnergyCost);
                }
                if (__instance.firstUseGrabbedObject != null)
                {
                    for (int j = 0; j < __instance.elecLines.Length; j++)
                    {
                        VFXElectricLine vfxelectricLine2 = __instance.elecLines[j];
                        vfxelectricLine2.origin = __instance.muzzle.position;
                        vfxelectricLine2.target = __instance.firstUseGrabbedObject.transform.position;
                        vfxelectricLine2.originVector = __instance.muzzle.forward;
                    }
                }
                __instance.maxMass = 1200f * QMod.config.propCannonMaxMass;
                __instance.pickupDistance = 18f * QMod.config.propCannonPickupDist;
                __instance.shootForce = 50f * QMod.config.propCannonShootForce;
                __instance.attractionForce = 140f * QMod.config.propCannonAttractionForce;
                __instance.massScalingFactor = 0.5f;
            }
        }
        [HarmonyPatch(typeof(PropulsionCannon))]
        [HarmonyPatch(nameof(PropulsionCannon.ValidateNewObject))]
        internal class patchPropCannonObjectOverride
        {
            [HarmonyPrefix]
            public static bool PropCannonPreFix(PropulsionCannon __instance, ref bool __result)
            {
                if(QMod.config.targetOverride)
                {
                    __result = true;
                    return false;
                }
                return true;
            }
        }
    }
}