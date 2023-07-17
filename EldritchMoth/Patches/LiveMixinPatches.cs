using EldritchMoth.Mono;
using HarmonyLib;
using System.Collections;
using UnityEngine;
using UWE;

namespace EldritchMoth.Patches
{
    [HarmonyPatch(typeof(LiveMixin))]
    internal class LiveMixinPatches
    {
        public const float damageToEnergyMult = 0.25f;
        public const float elecEffectCooldown = 5;

        private static GameObject _elecEffectPrefab;

        private static Vector3 _elecOffset = new(1, -0.2f, 0.4f);
        private static float elecEffectDuration = 0.5f;
        private static float timeLastElec = 0;

        [HarmonyPatch(nameof(LiveMixin.TakeDamage))]
        public static bool Prefix(LiveMixin __instance, float originalDamage, DamageType type, GameObject dealer)
        {
            if (!__instance.TryGetComponent(out EldritchMothMono mono)) 
                return true;

            if (!__instance.TryGetComponent(out EnergyMixin energyM)) 
                return true;


            var modifiedDam = DamageSystem.CalculateDamage(originalDamage, type, __instance.gameObject, dealer);
            var energyToConsume = modifiedDam * damageToEnergyMult;

            if (energyM.charge - energyToConsume <= energyM.capacity * 0.1f) 
                return true;//energy would go below the 10% threshold


            energyM.ConsumeEnergy(energyToConsume);
            CoroutineHost.StartCoroutine(PlayEnergyShieldEffects(__instance.gameObject));
            return false;
        }

        public static IEnumerator PlayEnergyShieldEffects(GameObject gameObject)
        {
            Utils.PlayFMODAsset(GetFmodAsset("event:/env/damage/shock"), Player.main.transform);
            if (!_elecEffectPrefab) yield return SetUpElecEffect();

            if (timeLastElec + elecEffectDuration >= Time.time || !Main.Config.useElecEffect) yield break;
            timeLastElec = Time.time;

            var elecEffect = GameObject.Instantiate(_elecEffectPrefab);
            elecEffect.SetActive(true);

            elecEffect.transform.parent = gameObject.transform;
            elecEffect.transform.position = gameObject.transform.position;
            elecEffect.transform.position += gameObject.transform.forward * _elecOffset.x;
            elecEffect.transform.position += gameObject.transform.up * _elecOffset.y;
            elecEffect.transform.position += gameObject.transform.right * (Random.Range(0, 6) % 2 == 0 ? _elecOffset.z : -_elecOffset.z);//sometimes spawns on left, sometimes on right

            GameObject.Destroy(elecEffect, elecEffectDuration);
        }

        public static IEnumerator SetUpElecEffect()
        {
            var request = PrefabDatabase.GetPrefabAsync("ff8e782e-e6f3-40a6-9837-d5b6dcce92bc");
            yield return request;
            request.TryGetPrefab(out var prefab);

            _elecEffectPrefab = GameObject.Instantiate(prefab);

            foreach (var rend in _elecEffectPrefab.GetComponentsInChildren<Renderer>(true))
            {
                rend.material.color = new Color(1, 0, 1, 0.7f);
                if (rend is ParticleSystemRenderer rad) rad.maxParticleSize /= 2;
            }
            _elecEffectPrefab.GetComponentInChildren<Light>().color = new Color(1, 0, 1, 0.7f);
            GameObject.Destroy(_elecEffectPrefab.GetComponent<DamagePlayerInRadius>());
            GameObject.Destroy(_elecEffectPrefab.GetComponent<PlayerDistanceTracker>());
            _elecEffectPrefab.SetActive(false);
        }

        public static FMODAsset GetFmodAsset(string audioPath)
        {
            FMODAsset asset = ScriptableObject.CreateInstance<FMODAsset>();
            asset.path = audioPath;
            return asset;
        }
    }
}
