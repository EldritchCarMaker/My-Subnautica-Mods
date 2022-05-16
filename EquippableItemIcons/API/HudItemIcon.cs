using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Logger = QModManager.Utility.Logger;

namespace EquippableItemIcons.API
{
    public class HudItemIcon
    {
        public string name;
        public Atlas.Sprite sprite;
        public TechType techType;

        public GameObject container;
        public GameObject itemIconObject;
        public uGUI_ItemIcon itemIcon;
        public bool active = false;

        //whether the mod handles icon animation itself or if its handled here
        public bool AutomaticSetup = true;

        public bool equipped = false;
        public EquipmentType equipmentType = EquipmentType.Chip;
        public bool iconActive = false;
        public bool InvertIcon = true;
        public bool playSounds = true;

        public delegate void ToggleEvent();
        public ToggleEvent Deactivate;
        public ToggleEvent Activate;
        public ToggleEvent OnEquip;
        public ToggleEvent OnUnEquip;

        public delegate bool AllowedEvent();
        public AllowedEvent CanActivate;
        public AllowedEvent IsIconActive;

        public FMODAsset ActivateSound;
        public FMODAsset DeactivateSound;

        public KeyCode activateKey = KeyCode.None;
        public Atlas.Sprite backgroundSprite;

        public float MaxCharge = 100;
        public float MinCharge = 0;
        public float ChargeRate = 20;
        public float DrainRate = 5;
        public float charge;
        public bool OnceOff = false;
        public float RechargeDelay = 0;


        private float TimeCharge = 0;

        public HudItemIcon(string name, Atlas.Sprite sprite, TechType itemTechType)
        {
            this.name = name;
            this.sprite = sprite;
            techType = itemTechType;
        }
        internal void makeIcon()
        {
            uGUI_QuickSlots quickSlots = uGUI.main.quickSlots;

            container = new GameObject(name + "Container");
            container.transform.SetParent(quickSlots.transform, false);
            container.layer = quickSlots.gameObject.layer;

            itemIconObject = new GameObject(name);
            itemIconObject.transform.SetParent(container.transform, false);
            itemIconObject.layer = quickSlots.gameObject.layer;

            itemIcon = itemIconObject.AddComponent<uGUI_ItemIcon>();
            itemIcon.Init(null, container.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
            itemIcon.SetForegroundSprite(sprite);
            itemIcon.SetForegroundBlending(Blending.AlphaBlend);

            itemIcon.CreateBackground();
            itemIcon.SetBackgroundSprite(backgroundSprite);

            if(ActivateSound == null)
            {
                ActivateSound = UtilityStuffs.Utility.GetFmodAsset("");
            }
            if (DeactivateSound == null)
            {
                DeactivateSound = UtilityStuffs.Utility.GetFmodAsset("");
            }

            if (InvertIcon)
                container.transform.eulerAngles = new Vector3(0, 180, 180);

            if (!AutomaticSetup)
            {
                iconActive = equipped;
                container.SetActive(iconActive);
                Registries.UpdatePositions();
                Logger.Log(Logger.Level.Info, $"Finished setup of {name}");
                return;
            }


            itemIcon.SetProgress(1, FillMethod.Vertical);

            equipped = UtilityStuffs.Utility.EquipmentHasItem(techType, equipmentType);

            iconActive = equipped;
            container.SetActive(iconActive);
            Registries.UpdatePositions();
            Logger.Log(Logger.Level.Info, $"Finished setup of {name}");
        }

        public void UpdateEquipped()
        {
            if (AutomaticSetup)
            {
                var temp = UtilityStuffs.Utility.EquipmentHasItem(techType, equipmentType);
                if (temp != equipped)
                {
                    Registries.UpdatePositions();
                    if (temp && OnEquip != null) OnEquip.Invoke();
                    else if(!temp && OnUnEquip != null) OnUnEquip.Invoke();
                }
                equipped = temp;
                container.transform.eulerAngles = new Vector3(0, 180, 180);//for some reason the angle would be off unless I set it here
            }
            Registries.UpdatePositions();
        }
        public void Update()
        {
            iconActive = IsIconActive != null? IsIconActive.Invoke() : equipped;
            if (container)
                container.SetActive(iconActive);

            if (!equipped)
            {
                if (active)
                {
                    HandleDeactivation();
                }
                return;
            }


            if (Input.GetKeyDown(activateKey) && CanActivate.Invoke())
            {
                if (!active)
                {
                    HandleActivation();
                }
                else
                {
                    HandleDeactivation();
                }
            }
            if(OnceOff)
            {
                if(Time.time >= TimeCharge)
                {
                    charge = Mathf.Min(charge + (ChargeRate * Time.deltaTime), MaxCharge);
                }
            }
            else
            {
                if (active)
                {
                    charge = Mathf.Max(charge - (DrainRate * Time.deltaTime), 0);
                    if (charge <= 0)
                    {
                        HandleDeactivation();
                    }
                }
                else if (charge < MaxCharge)
                {
                    charge = Mathf.Min(charge + (ChargeRate * Time.deltaTime), MaxCharge);
                }
            }
            UpdateFill();
        }
        public void UpdateFill()
        {
            if (this.itemIcon == null || this.itemIcon.foreground == null || this.itemIcon.background == null) return;

            this.itemIcon.SetProgress(0, FillMethod.Vertical);

            this.itemIcon.background.material.SetFloat(ShaderPropertyID._FillValue, (100f / (this.MaxCharge / this.charge)) - 50f);
            //percent minus 50 because for some reason this value is offset. 50 is max, -50 is minimum. 
            this.itemIcon.foreground.material.SetFloat(ShaderPropertyID._FillValue, (100f / (this.MaxCharge / this.charge)) - 50f);
        }
        private void HandleActivation()
        {
            Activate?.Invoke();
            if (OnceOff)
            {
                charge -= DrainRate;
                TimeCharge = Time.time + RechargeDelay;
            }
            else
            {
                active = true;
            }
            if (QMod.config.SoundsActive && playSounds && ActivateSound)
            {
                Utils.PlayFMODAsset(ActivateSound);
            }
        }
        private void HandleDeactivation()
        {
            Deactivate?.Invoke();
            if (OnceOff)
            {
                //I'm sure I'll add stuff here later
            }
            else
            {
                active = false;
            }
            if(QMod.config.SoundsActive && playSounds && DeactivateSound)
            {
                Utils.PlayFMODAsset(DeactivateSound);
            }
        }
    }
}
