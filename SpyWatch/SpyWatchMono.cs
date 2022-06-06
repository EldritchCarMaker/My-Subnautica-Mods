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

        public ActivatedEquippableItem itemIcon;

        internal int FixedFramesSinceCheck = 0;

        private Player player;
        private Dictionary<Material[], GameObject> materials = new Dictionary<Material[], GameObject>();
        public void Awake()
        {
            player = GetComponent<Player>();
            foreach(Renderer renderer in player.GetComponentsInChildren<Renderer>())
            {
                materials.Add(renderer.materials, renderer.gameObject);
            }
            var sprite = ImageUtils.LoadSpriteFromFile(Path.Combine(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets"), "SpyWatchIconRotate.png"));

            itemIcon = new ActivatedEquippableItem("spyWatchIcon", sprite, SpyWatchItem.thisTechType);
            itemIcon.Deactivate += Deactivate;
            itemIcon.Activate += Activate;
            itemIcon.activateKey = QMod.config.SpyWatchKey;
            itemIcon.backgroundSprite = sprite;
            itemIcon.ActivateSound = Utility.GetFmodAsset(EnableCloakSoundPath);
            itemIcon.DeactivateSound = Utility.GetFmodAsset(DisableCloakSoundPath);
            Registries.RegisterHudItemIcon(itemIcon);

            //CoroutineHost.StartCoroutine(SetUpIcons());
        }
        public void Deactivate()
        {
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
            foreach (Renderer renderer in player.transform.Find("body").GetComponentsInChildren<Renderer>())
            {
                if (renderer.GetComponentInParent<PlayerTool>() != null) continue;

                else if ((bool)(renderer.transform.parent?.name.Contains("PDA"))) continue;
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
