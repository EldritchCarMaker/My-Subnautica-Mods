using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CameraDroneDefenseUpgrade
{
    internal static class ZapFunctionality//thanks prime lol
    {
        private static GameObject seamothElectricalDefensePrefab = null;
        public static GameObject ElectricalDefensePrefab => seamothElectricalDefensePrefab ??
            (seamothElectricalDefensePrefab = CraftData.GetPrefabForTechType(TechType.Seamoth)?.GetComponent<SeaMoth>().seamothElectricalDefensePrefab);

        private const float EnergyCostPerZap = 5;
        private const float ZapPower = 6f;
        private const float DamageMultiplier = 30f;
        private const float BaseCharge = 2f;
        private const float BaseRadius = 1f;

        private const float DirectZapDamage = (BaseRadius + ZapPower * BaseCharge) * DamageMultiplier * 0.5f;
        // Calculations and initial values based off ElectricalDefense component

        public const float ZapCooldown = 10f;
        public static float timeNextZap = 0;

        public static bool AbleToZap(MapRoomCamera camera)
        {
            if (GameModeUtils.RequiresPower() && camera.energyMixin.charge < EnergyCostPerZap)
                return false;

            return true;
        }
        public static bool IsTargetValid(GameObject target)
        {
            if (target == null)
                return false;

            if (!target.TryGetComponent(out LiveMixin mixin))
                return false;

            return mixin.IsAlive();
        }

        public static bool Zap(MapRoomCamera camera, GameObject obj)
        {
            if (Time.time < timeNextZap)
                return true;

            if (!AbleToZap(camera))
                return false;

            ZapRadius(camera);

            if (!IsTargetValid(obj))
                return true;//true because I still want stalker to let go even if the stalker wasn't zapped directly

            ZapCreature(camera, obj);
            timeNextZap = Time.time + ZapCooldown;
            return true;
        }

        private static void ZapRadius(MapRoomCamera originCamera)
        {
            if (originCamera == null)
                return;

            GameObject gameObject = Utils.SpawnZeroedAt(ElectricalDefensePrefab, originCamera.transform, false);
            ElectricalDefense defenseComponent = gameObject.GetComponent<ElectricalDefense>();
            defenseComponent.charge = ZapPower;
            defenseComponent.chargeScalar = ZapPower;
            defenseComponent.radius *= ZapPower;
            defenseComponent.chargeRadius *= ZapPower;

            if (GameModeUtils.RequiresPower())
                originCamera.energyMixin.ConsumeEnergy(EnergyCostPerZap);
        }

        private static void ZapCreature(MapRoomCamera originCamera, GameObject target)
        {
            if (originCamera == null || target == null)
                return;

            target.GetComponent<LiveMixin>().TakeDamage(DirectZapDamage, default, DamageType.Electrical, originCamera.gameObject);

            if (GameModeUtils.RequiresPower())
                originCamera.energyMixin.ConsumeEnergy(EnergyCostPerZap);
        }
    }
}
