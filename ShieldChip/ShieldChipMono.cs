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

namespace ShieldChip
{
    internal class ShieldChipMono : MonoBehaviour
    {
        public HudItemIcon hudItemIcon = new HudItemIcon("ShieldChipIcon", ImageUtils.LoadSpriteFromFile(Path.Combine(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets"), "ShieldChipIconRotate.png")), ShieldChipItem.thisTechType);
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

            hudItemIcon.name = "ShieldChipIcon";
            var sprite = ImageUtils.LoadSpriteFromFile(Path.Combine(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets"), "ShieldChipIconRotate.png"));
            hudItemIcon.sprite = sprite;
            hudItemIcon.backgroundSprite = sprite;
            hudItemIcon.Activate += Activate;
            hudItemIcon.Deactivate += Deactivate;
            hudItemIcon.CanActivate += CanActivate;
            hudItemIcon.IsIconActive += IsIconActive;
            hudItemIcon.activateKey = QMod.config.ShieldChipKey;
            hudItemIcon.techType = ShieldChipItem.thisTechType;
            Registries.RegisterHudItemIcon(hudItemIcon);

            SetUpShield();
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
        
        public void Update()
        {
            hudItemIcon.Update();

            if (shieldFX != null && shieldFX.gameObject.activeSelf)
            {
                shieldImpactIntensity = Mathf.MoveTowards(shieldImpactIntensity, 0f, Time.deltaTime / 4f);
                shieldIntensity = Mathf.MoveTowards(shieldIntensity, shieldGoToIntensity, Time.deltaTime / 2f);
                shieldFX.material.SetFloat(ShaderPropertyID._Intensity, shieldIntensity);
                shieldFX.material.SetFloat(ShaderPropertyID._ImpactIntensity, shieldImpactIntensity);

                if (Mathf.Approximately(shieldIntensity, 0f) && shieldGoToIntensity == 0f)
                {
                    shieldFX.gameObject.SetActive(false);
                }
            }
        }
        public void FixedUpdate()
        {
            if(FixedUpdatesSinceCheck > 20)
            {
                FixedUpdatesSinceCheck = 0;
                hudItemIcon.UpdateEquipped();
            }
            FixedUpdatesSinceCheck++;
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
        public bool CanActivate()
        {
            return !player.isPiloting && !player.GetPDA().isOpen;
        }
        public bool IsIconActive()
        {
            return hudItemIcon.equipped;
        }
    }
}
