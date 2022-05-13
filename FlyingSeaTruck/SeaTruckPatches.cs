using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FlyingSeaTruck
{
    [HarmonyPatch(typeof(SeaTruckUpgrades))]
    internal class SeaTruckPatches
    {
        public static float originalGrav = 9.81f;
		public static List<SeaTruckMotor> flyingSeaTrucks = new List<SeaTruckMotor>();


        [HarmonyPatch(nameof(SeaTruckUpgrades.OnUpgradeModuleChange))]
        public static void Postfix(SeaTruckUpgrades __instance, TechType techType, bool added)
        {
            if (techType == SeatruckFlightModule.thisTechType)
            {
				SetFlying(__instance.motor, added);

				if(added) flyingSeaTrucks.Add(__instance.motor);
                else flyingSeaTrucks.Remove(__instance.motor);
            }
        }
        
        [HarmonyPatch(typeof(SeaTruckMotor))]
        [HarmonyPatch(nameof(SeaTruckMotor.FixedUpdate))]
        public static void Prefix(SeaTruckMotor __instance)
        {
			if (!(__instance.transform.position.y < Ocean.GetOceanLevel()) && __instance.useRigidbody != null && __instance.IsPowered() && !__instance.IsBusyAnimating())
			{
				if (__instance.piloting)
				{
					Vector3 vector = (AvatarInputHandler.main.IsEnabled() || __instance.inputStackDummy.activeInHierarchy) ? GameInput.GetMoveDirection() : Vector3.zero;
					Int2 leverDirection;
					if (vector.x > 0f)
					{
						leverDirection.x = 1;
					}
					else if (vector.x < 0f)
					{
						leverDirection.x = -1;
					}
					else
					{
						leverDirection.x = 0;
					}
					if (vector.z > 0f)
					{
						leverDirection.y = 1;
					}
					else if (vector.z < 0f)
					{
						leverDirection.y = -1;
					}
					else
					{
						leverDirection.y = 0;
					}
					__instance.leverDirection = leverDirection;
					if (__instance.afterBurnerActive)
					{
						vector.z = 1f;
					}
					vector = vector.normalized;
					Vector3 a = MainCameraControl.main.rotation * vector;
					float num = 1f / Mathf.Max(1f, __instance.GetWeight() * 0.35f) * __instance.acceleration;
					if (__instance.afterBurnerActive)
					{
						num += 7f;
					}
					__instance.useRigidbody.AddForce(num * a, ForceMode.Acceleration);
					if (__instance.relay && vector != Vector3.zero)
					{
						float num2;
						__instance.relay.ConsumeEnergy(Time.deltaTime * __instance.powerEfficiencyFactor * 0.12f, out num2);
					}
				}
				__instance.StabilizeRoll();
			}
		}
		[HarmonyPatch(typeof(SeaTruckMotor), nameof(SeaTruckMotor.CanTurn))]
		[HarmonyPostfix]
		public static void Post(SeaTruckMotor __instance, ref bool __result)
        {
			__result = true;
		}
		[HarmonyPatch(typeof(SeaTruckMotor), nameof(SeaTruckMotor.StartPiloting))]
		[HarmonyPostfix]
		public static void asd(SeaTruckMotor __instance)
        {
			if(flyingSeaTrucks.Contains(__instance))
            {
				SetFlying(__instance, true);
            }
        }
		[HarmonyPatch(typeof(SeaTruckMotor), nameof(SeaTruckMotor.StopPiloting))]
		[HarmonyPostfix]
		public static void asdd(SeaTruckMotor __instance)
		{
			if(QMod.config.SeaTruckFalls)
				SetFlying(__instance, false);
		}

		public static void SetFlying(SeaTruckMotor __instance, bool flying)
        {
			var com = __instance.gameObject.GetComponent<WorldForces>();
			com.aboveWaterGravity = flying ? 0 : originalGrav;

			com.aboveWaterDrag = com.underwaterDrag;
			__instance.aboveWaterDrag = com.underwaterDrag;
		}
    }
}
