using CameraDroneUpgrades.API;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UWE;

namespace CameraDroneShieldUpgrade
{
    internal class ShieldFunctionality
    {
        public CameraDroneUpgrade upgrade;

        private MeshRenderer shieldFX;
        private float shieldIntensity;
        private float shieldImpactIntensity;
        private float shieldGoToIntensity;

        private FMODAsset shield_on_loop;
        private FMOD_CustomEmitter sfx;

        public void SetUp()
        {
            upgrade.update += Update;
            upgrade.activate += Activate;
            upgrade.deactivate += Deactivate;
            upgrade.key = QMod.config.shieldKey;

            shield_on_loop = ScriptableObject.CreateInstance<FMODAsset>();
            shield_on_loop.name = "shield_on_loop";
            shield_on_loop.path = "event:/sub/cyclops/shield_on_loop";
            sfx = upgrade.camera.gameObject.AddComponent<FMOD_CustomEmitter>();
            sfx.asset = shield_on_loop;
            sfx.followParent = true;
            CoroutineHost.StartCoroutine(WaitForLMP());
        }
        public IEnumerator WaitForLMP()
        {
            yield return new WaitUntil(() => LightmappedPrefabs.main);
            LightmappedPrefabs.main.RequestScenePrefab("Cyclops", new LightmappedPrefabs.OnPrefabLoaded(OnSubPrefabLoaded));
        }
        public void OnSubPrefabLoaded(GameObject prefab)
        {
            SubRoot sub = prefab.GetComponent<SubRoot>();

            shieldFX = GameObject.Instantiate(sub.shieldFX, upgrade.camera.transform);

            shieldFX.gameObject.SetActive(false);

            Utils.ZeroTransform(shieldFX.transform);
            shieldFX.gameObject.transform.parent = upgrade.camera.transform;

            shieldFX.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
            shieldFX.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

        }
        public void Update()
        {
            if (shieldFX == null || shieldFX.material == null || shieldFX.gameObject == null) return;


            shieldIntensity = Mathf.MoveTowards(shieldIntensity, shieldGoToIntensity, Time.deltaTime / 2f);
            shieldFX.material.SetFloat(ShaderPropertyID._Intensity, shieldIntensity);
            shieldFX.material.SetFloat(ShaderPropertyID._ImpactIntensity, shieldImpactIntensity);
            if (Mathf.Approximately(shieldIntensity, 0f) && shieldGoToIntensity == 0f)
            {
                shieldFX.gameObject.SetActive(false);
            }
        }
        public void Deactivate()
        {
            sfx.Stop();

            shieldGoToIntensity = 0f;
            upgrade.camera.liveMixin.shielded = false;
        }
        public void Activate()
        {
            sfx.Play();

            shieldFX.gameObject.SetActive(true);
            upgrade.camera.liveMixin.shielded = true;
            shieldGoToIntensity = 1f;
        }
    }
}
