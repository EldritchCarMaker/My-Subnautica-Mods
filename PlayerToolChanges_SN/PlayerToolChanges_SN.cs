using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;

namespace PlayerToolChanges
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
                else if (__instance.GetType() == typeof(AirBladder))
                {
                    AirBladder airBladder = __instance as AirBladder;
                    float force = QMod.config.AirBladderForce;
#if SN1
                    airBladder.forceConstant = force * 0.4f;
#else
                    airBladder.buoyancyForce = force * 0.8f;
#endif
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
                else if(__instance.GetType() == typeof(StasisRifle))
                {
                    StasisRifle stasisRifle = __instance as StasisRifle;

                    float chargeDurationMultiplier = QMod.config.stasisRifleChargeRate;
                    float EnergyDrainMultiplier = QMod.config.stasisRifleEnergyCost;

                    stasisRifle.chargeDuration = 3f / chargeDurationMultiplier;
                    stasisRifle.energyCost = 5f / EnergyDrainMultiplier;
                }
                else if (__instance.GetType() == typeof(ScannerTool))
                {
                    ScannerTool scannerTool = __instance as ScannerTool;

                    float scannerEnergyDrain = QMod.config.scannerEnergyCost;

                    scannerTool.powerConsumption = scannerEnergyDrain * 0.2f;
                }
            }
            [HarmonyPatch(typeof(StasisSphere), nameof(StasisSphere.Shoot))]
            [HarmonyPrefix]
            public static void stasisSpherePatch(StasisSphere __instance)
            {
                __instance.maxRadius = QMod.config.stasisRifleBubbleRadius * 10f;
                __instance.maxTime = QMod.config.stasisRifleBubbleDuration * 20f;
            }
        }
        [HarmonyPatch]
        internal class patchPropCannon
        {
            [HarmonyPatch(typeof(PropulsionCannon), nameof(PropulsionCannon.Update))]
            [HarmonyPrefix]
            public static bool PropCannonUpdateEnergyChanger(PropulsionCannon __instance)
            {
                //I know I shouldn't be returning false, but it's way easier than doing my first transpiler
                //plus I only return false if the energy config is different from 1
                if (QMod.config.propCannonEnergyCost == 1)
                {
                    return true;
                }
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
                return false;
            }
            [HarmonyPatch(typeof(PropulsionCannon), nameof(PropulsionCannon.OnShoot))]
            [HarmonyPrefix]
            public static bool PropCannonShootEnergyChanger(PropulsionCannon __instance, ref bool __result)
            {
                //I know I shouldn't be returning false, but it's way easier than doing my first transpiler
                //plus I only return false if the energy config is different from 1
                if (QMod.config.propCannonEnergyCost == 1)
                {
                    return true;
                }
                if (__instance.grabbedObject != null)
                {
                    float num;
                    float num2;
                    __instance.energyInterface.GetValues(out num, out num2);
                    float d = Mathf.Min(1f, num / 4f);
                    Rigidbody component = __instance.grabbedObject.GetComponent<Rigidbody>();
                    float d2 = 1f + component.mass * __instance.massScalingFactor;
                    Vector3 vector = MainCamera.camera.transform.forward * __instance.shootForce * d / d2;
                    Vector3 velocity = component.velocity;
                    if (Vector3.Dot(velocity, vector) < 0f)
                    {
                        component.velocity = vector;
                    }
                    else
                    {
                        component.velocity = velocity * 0.3f + vector;
                    }
                    __instance.grabbedObject.GetComponent<PropulseCannonAmmoHandler>().OnShot(false);
                    __instance.launchedObjects.Add(__instance.grabbedObject);
                    __instance.grabbedObject = null;
                    __instance.energyInterface.ConsumeEnergy(4f * QMod.config.propCannonShootEnergyCost);
                    Utils.PlayFMODAsset(__instance.shootSound, __instance.transform, 20f);
                    __instance.fxControl.Play(0);
                }
                else
                {
                    GameObject gameObject = __instance.TraceForGrabTarget();
                    if (gameObject != null)
                    {
                        __instance.GrabObject(gameObject);
                    }
                    else
                    {
                        Utils.PlayFMODAsset(__instance.grabFailSound, __instance.transform, 20f);
                    }
                }
                __result = true;
                return false;
            }

            [HarmonyPatch(typeof(PropulsionCannon), nameof(PropulsionCannon.GrabObject))]
            [HarmonyPrefix]
            public static void propCannonStatChanger(PropulsionCannon __instance)
            {
                __instance.maxMass = 1200f * QMod.config.propCannonMaxMass;
                __instance.pickupDistance = 18f * QMod.config.propCannonPickupDist;
                __instance.shootForce = 50f * QMod.config.propCannonShootForce;
                __instance.attractionForce = 140f * QMod.config.propCannonAttractionForce;
                __instance.massScalingFactor = 0.02f * QMod.config.propCannonMassScalingFactorMultiplier;
            }
            [HarmonyPatch(typeof(PropulsionCannon), nameof(PropulsionCannon.Start))]
            [HarmonyPrefix]
            public static void propCannonStartStatChanger(PropulsionCannon __instance)
            {
                __instance.maxMass = 1200f * QMod.config.propCannonMaxMass;
                __instance.pickupDistance = 18f * QMod.config.propCannonPickupDist;
                __instance.shootForce = 50f * QMod.config.propCannonShootForce;
                __instance.attractionForce = 140f * QMod.config.propCannonAttractionForce;
                __instance.massScalingFactor = 0.02f * QMod.config.propCannonMassScalingFactorMultiplier;
            }
            [HarmonyPatch(typeof(PropulsionCannon), nameof(PropulsionCannon.ValidateNewObject))]
            [HarmonyPrefix]
            public static bool PropCannonOverride(PropulsionCannon __instance, ref bool __result)
            {
                if (QMod.config.targetOverride)
                {
                    __result = true;
                    return false;
                }
                return true;
            }
        }
        [HarmonyPatch]
        public static class PDAScanPatches
        {
            [HarmonyPatch(typeof(PDAScanner), nameof(PDAScanner.Scan))]
            [HarmonyPrefix]
            public static void Prefix()
            {
                TechType techType = PDAScanner.scanTarget.techType;
                PDAScanner.EntryData entryData = PDAScanner.GetEntryData(techType);
                if (entryData != null && QMod.config.ScanDuration != 2)
                {
                    entryData.scanTime = QMod.config.ScanDuration;
                }
            }
        }
        //patch constructable.getconstructinterval
        //should let the habitat builder build faster

        //patch the pipes thing
        //getpipeposition mathf.clamp make transpiler for that
    }
}