using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UWE;

namespace CameraDroneDefenseUpgrade
{
    internal static class ZapFunctionality//thanks prime lol
    {
        private static GameObject seamothElectricalDefensePrefab = null;

        public static GameObject ElectricalDefensePrefab =>
#if SN1
            seamothElectricalDefensePrefab ??
            (seamothElectricalDefensePrefab = CraftData.GetPrefabForTechType(TechType.Seamoth)?.GetComponent<SeaMoth>().seamothElectricalDefensePrefab);
#else
            seamothElectricalDefensePrefab;
#endif

        private const float EnergyCostPerZap = 5;
        private const float ZapPower = 6f;
        private const float BaseCharge = 2f;
        private const float BaseRadius = 1f;

        public const float ZapCooldown = 10f;
        public static float timeNextZap = 0;
        private static float DamageMultiplier => QMod.config.damageAmount;
        private static float DirectZapDamage = (BaseRadius + ZapPower * BaseCharge) * DamageMultiplier * 0.5f;
        // Calculations and initial values based off ElectricalDefense component

        public static bool AbleToZap(MapRoomCamera camera)
        {

            if (
#if SN
                GameModeUtils.RequiresPower()
#else
                GameModeManager.GetOption<bool>(GameOption.TechnologyRequiresPower)
#endif
                && camera.energyMixin.charge < EnergyCostPerZap)
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

        public static IEnumerator UpdateDefensePrefab()
        {
            if (seamothElectricalDefensePrefab) yield break;

            var task = CraftData.GetPrefabForTechTypeAsync(TechType.Seamoth);
            yield return task;
            var prefab = task.GetResult();

            seamothElectricalDefensePrefab = prefab?.GetComponent<SeaMoth>().seamothElectricalDefensePrefab;
        }

        public static bool Zap(MapRoomCamera camera, GameObject obj)
        {
#if !SN1
            CoroutineHost.StartCoroutine(UpdateDefensePrefab());
#endif
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

            if (
#if SN
                GameModeUtils.RequiresPower()
#else
                GameModeManager.GetOption<bool>(GameOption.TechnologyRequiresPower)
#endif
                )
                originCamera.energyMixin.ConsumeEnergy(EnergyCostPerZap);
        }

        private static void ZapCreature(MapRoomCamera originCamera, GameObject target)
        {
            if (originCamera == null || target == null)
                return;

            target.GetComponent<LiveMixin>().TakeDamage(DirectZapDamage, default, DamageType.Electrical, originCamera.gameObject);

            if (
#if SN
                GameModeUtils.RequiresPower()
#else
                GameModeManager.GetOption<bool>(GameOption.TechnologyRequiresPower)
#endif
                )
                originCamera.energyMixin.ConsumeEnergy(EnergyCostPerZap);
        }
    }
}
