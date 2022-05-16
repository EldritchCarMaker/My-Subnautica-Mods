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
using UnityEngine.UI;
using UtilityStuffs;
using Logger = QModManager.Utility.Logger;
using UWE;
using EquippableItemIcons.API;

namespace SpyWatch
{
    internal class SpyWatchMono : MonoBehaviour
    {
        //private const string EnableCloakSoundPath = "event:/sub/cyclops/start";
        //bit too heavy sounding, maybe good for cyclops cloak but ideally find something more lightweight 
        //private const string EnableCloakSoundPath = "event:/sub/base/power_off";

        private const string EnableCloakSoundPath = "event:/sub/cyclops/install_mod";//found it
        private const string DisableCloakSoundPath = "event:/tools/builder/remove";

        public HudItemIcon itemIcon;

        internal int FixedFramesSinceCheck = 0;

        private Player player;
        private Dictionary<Material[], GameObject> materials = new Dictionary<Material[], GameObject>();
        private GameObject container;
        //private uGUI_Icon WatchIcon;
        private uGUI_ItemIcon WatchIcon;
        public void Awake()
        {
            player = GetComponent<Player>();
            foreach(Renderer renderer in player.GetComponentsInChildren<Renderer>())
            {
                materials.Add(renderer.materials, renderer.gameObject);
            }
            var sprite = ImageUtils.LoadSpriteFromFile(Path.Combine(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets"), "SpyWatchIconRotate.png"));

            itemIcon = new HudItemIcon("spyWatchIcon", sprite, SpyWatchItem.thisTechType);
            itemIcon.Deactivate += Deactivate;
            itemIcon.Activate += Activate;
            itemIcon.activateKey = QMod.config.SpyWatchKey;
            itemIcon.CanActivate += CanActivate;
            itemIcon.backgroundSprite = sprite;
            Registries.RegisterHudItemIcon(itemIcon);

            itemIcon.UpdateEquipped();

            //CoroutineHost.StartCoroutine(SetUpIcons());
        }
        public bool CanActivate()
        {
            return !player.isPiloting && !player.GetPDA().isOpen;
        }
        private IEnumerator SetUpIcons()
        {


            yield return new WaitUntil(() => uGUI.main != null && uGUI.main.quickSlots != null && uGUI.main.quickSlots.gameObject != null && uGUI.main.quickSlots.icons != null);
            uGUI_QuickSlots quickSlots = uGUI.main.quickSlots;

            container = new GameObject("WatchIconContainer");
            container.transform.SetParent(quickSlots.transform, false);
            container.layer = quickSlots.gameObject.layer;
            container.transform.localPosition = quickSlots.GetPosition(quickSlots.icons.Length) + new Vector2(20, 0);
            container.transform.eulerAngles = new Vector3(180, 0, 0);


            GameObject bg_object = new GameObject("WatchIcon");
            bg_object.transform.SetParent(container.transform, false);
            bg_object.layer = quickSlots.gameObject.layer;

            WatchIcon = bg_object.AddComponent<uGUI_ItemIcon>();
            WatchIcon.Init(null, container.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
            //WatchIcon.SetForegroundSprite(sprite);
            WatchIcon.SetForegroundBlending(Blending.AlphaBlend);

            WatchIcon.CreateBackground();
            //WatchIcon.SetBackgroundSprite(sprite);


            WatchIcon.SetProgress(1, FillMethod.Vertical);

            //unfortunately the games method to do this seems bugged, need to do it myself
            //MaterialExtensions.SetKeyword(WatchIcon.background.material, "FILL_VERTICAL", true);
            //MaterialExtensions.SetKeyword(WatchIcon.foreground.material, "FILL_VERTICAL", true);




            /*
            WatchIcon = bg_object.AddComponent<uGUI_Icon>();
            WatchIcon.sprite = SpyWatchItem.sprite;
            WatchIcon.rectTransform.offsetMin = new Vector2(-80, -80);
            WatchIcon.rectTransform.offsetMax = new Vector2(80, 80);
            */


            container.SetActive(false);
        }

        public void Update()
        {
            itemIcon.Update();
        }
        public void FixedUpdate()
        {
            if (FixedFramesSinceCheck >= 10)
            {
                itemIcon.UpdateEquipped();
                FixedFramesSinceCheck = 0;
            }

            FixedFramesSinceCheck++;
        }
        public void Deactivate()
        {
            Utils.PlayFMODAsset(Utility.GetFmodAsset(DisableCloakSoundPath), transform);
            foreach (Renderer renderer in player.GetComponentsInChildren<Renderer>())
            {

                foreach (KeyValuePair<Material[], GameObject> pair in materials)
                {
                    if (pair.Value.GetInstanceID() == renderer.gameObject.GetInstanceID())
                    {
                        renderer.materials = pair.Key;
                    }
                }
            }
        }
        public void Activate()
        {
            Utils.PlayFMODAsset(Utility.GetFmodAsset(EnableCloakSoundPath), transform);
            foreach (Renderer renderer in player.transform.Find("body").GetComponentsInChildren<Renderer>())
            {
                /*
                if (!materials.ContainsValue(renderer.gameObject))
                {
                    materials.Add(renderer.materials, renderer.gameObject);
                }
                else
                {
                    materials[renderer.materials] = renderer.gameObject;
                }
                
                Transform parent = renderer.transform;
                int count = 0;
                while(parent != null && count < 6)
                {
                    if(parent.gameObject.name.Contains("FX"))
                    {
                        continue;
                    }
                    count++;
                    parent = parent.parent;
                }
                */

                var mats = renderer.materials;
                for (int i = 0; i < mats.Length; i++)
                {
                    mats[i] = QMod.CloakMaterial;
                }

                renderer.materials = mats;
            }
        }
    }
}
