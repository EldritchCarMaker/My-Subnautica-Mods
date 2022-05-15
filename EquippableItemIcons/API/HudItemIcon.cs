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
        public Atlas.Sprite backgroundSprite;

        public GameObject container;
        public GameObject itemIconObject;
        public uGUI_ItemIcon itemIcon;

        //whether the mod handles icon animation itself or if its handled here
        public bool AutomaticSetup = true;

        public bool equipped = false;
        public TechType techType;
        public EquipmentType equipmentType = EquipmentType.Chip;
        public bool active = false;
        public bool iconActive = false;

        public delegate void ToggleEvent();
        public ToggleEvent Deactivate;
        public ToggleEvent Activate;
        public ToggleEvent OnEquip;
        public ToggleEvent OnUnEquip;

        public delegate bool AllowedEvent();
        public AllowedEvent CanActivate;
        public AllowedEvent IsIconActive;

        public KeyCode activateKey = KeyCode.None;

        public float MaxCharge = 100;
        public float MinCharge = 0;
        public float ChargeRate = 20;
        public float DrainRate = 5;
        public float charge;


        public void makeIcon()
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

            if (!AutomaticSetup)
            {
                iconActive = equipped;
                container.SetActive(iconActive);
                Registries.RegisterHudItemIcon(this);
                Logger.Log(Logger.Level.Info, $"Finished setup of {name}");
                return;
            }


            container.transform.eulerAngles = new Vector3(0, 180, 180);

            itemIcon.SetProgress(1, FillMethod.Vertical);

            equipped = UtilityStuffs.Utility.EquipmentHasItem(techType, equipmentType);

            iconActive = equipped;
            container.SetActive(iconActive);
            Registries.RegisterHudItemIcon(this);
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
                    Deactivate.Invoke();
                    active = false;
                }
                return;
            }


            if (Input.GetKeyDown(activateKey) && CanActivate.Invoke())
            {
                if (!active)
                {
                    Activate.Invoke();
                    active = true;
                }
                else
                {
                    Deactivate.Invoke();
                    active = false;
                }
            }

            if (active)
            {
                charge = Mathf.Max(charge - (DrainRate * Time.deltaTime), 0);
                if (charge <= 0)
                {
                    Deactivate.Invoke();
                    active = false;
                }
            }
            else if (charge < MaxCharge)
            {
                charge = Mathf.Min(charge + (ChargeRate * Time.deltaTime), MaxCharge);
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
    }
}
