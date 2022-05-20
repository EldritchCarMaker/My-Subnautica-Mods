using EquippableItemIcons.API;
using SMLHelper.V2.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UWE;
using Logger = QModManager.Utility.Logger;

namespace ShieldSuit
{
    internal class ShieldSuitMono : MonoBehaviour
    {
        public HudItemIcon hudItemIcon = new HudItemIcon("ShieldSuitItem", ImageUtils.LoadSpriteFromFile(Path.Combine(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets"), "ShieldSuitIconRotate.png")), ShieldSuitItem.thisTechType);
        public Player player;
        public int FixedUpdatesSinceCheck = 0;

        private MeshRenderer shieldFX;
        private float shieldIntensity;
        private float shieldImpactIntensity;
        private float shieldGoToIntensity;

        private FMODAsset shield_on_loop;
        private FMOD_CustomEmitter sfx;

        public void Awake()
        {
            player = GetComponent<Player>();
            SetUpShield();

            var sprite = ImageUtils.LoadSpriteFromFile(Path.Combine(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets"), "ShieldSuitIconRotate.png"));
            hudItemIcon.sprite = sprite;
            hudItemIcon.backgroundSprite = sprite;
            hudItemIcon.equipmentType = EquipmentType.Body;
            hudItemIcon.Activate += Activate;
            hudItemIcon.Deactivate += Deactivate;
            hudItemIcon.activateKey = QMod.config.ShieldSuitKey;
            hudItemIcon.DeactivateSound = null;

            Registries.RegisterHudItemIcon(hudItemIcon);

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
        public void SetUpShield()
        {
            shield_on_loop = ScriptableObject.CreateInstance<FMODAsset>();
            shield_on_loop.name = "shield_on_loop";
            shield_on_loop.path = "event:/sub/cyclops/shield_on_loop";
            sfx = gameObject.AddComponent<FMOD_CustomEmitter>();
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

            shieldFX = Instantiate(sub.shieldFX,transform);

            shieldFX.gameObject.SetActive(false);

            Utils.ZeroTransform(shieldFX.transform);
            shieldFX.gameObject.transform.parent = MainCamera.camera.transform;

            shieldFX.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
            shieldFX.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            
        }
        public void Deactivate()
        {
            sfx.Stop();

            shieldGoToIntensity = 0f;
            player.liveMixin.shielded = false;
        }
        public void Activate()
        {
            sfx.Play();

            shieldFX.gameObject.SetActive(true);
            player.liveMixin.shielded = true;
            shieldGoToIntensity = 1f;
        }
    }
}
