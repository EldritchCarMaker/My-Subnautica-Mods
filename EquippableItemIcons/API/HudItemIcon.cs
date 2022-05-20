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
        public string name;//self explanatory
        public Atlas.Sprite sprite;//foreground sprite of item icon
        public TechType techType;//used to determine if item is equipped or not

        public GameObject container;//try to avoid touching these if AutomaticSetup is true, otherwise go ham
        public GameObject itemIconObject;
        public uGUI_ItemIcon itemIcon;
        public bool active = false;//whether the item is currently being used, not the same as equipped or icon active. Also avoid touching if using AutomaticSetup

        //whether we handle the item's charge, activation, and icon animation, or your mod does
        public bool AutomaticSetup = true;

        public bool equipped = false;//if item is equipped by player
        public EquipmentType equipmentType = EquipmentType.Chip;//the item slot its equipped into
        public bool iconActive = false;//if the icon is currently showing
        public bool InvertIcon = true;//if the container should be rotated upside down, used so that the animation fades from top to bottom rather than bottom to top. If true, the sprite should be rotated 180 degrees to compensate
        public bool playSounds = true;//whether this item plays sounds or not, overriden by the mod config
        public bool fadeBackground = true;//whether the background sprite also fades, or if its only the foreground sprite

        public delegate void ToggleEvent();//any event with returns void
        public ToggleEvent Deactivate;//stop the functionality of item
        public ToggleEvent Activate;//the actual functionality of item, when its activated what does it do?
        public ToggleEvent OnEquip;//in case you need to do something special when item is equipped
        public ToggleEvent OnUnEquip;// case you need to do something special when item is unequipped

        public delegate bool AllowedEvent();//any event that returns a bool
        public AllowedEvent CanActivate;//used to tell if the item can currently be activated or not. Has a default, but good for extra conditions specific to the item
        public AllowedEvent IsIconActive;//used for if there's a specific condition for when the icon should/shouldn't be active, has a default 

        public FMODAsset ActivateSound = UtilityStuffs.Utility.GetFmodAsset("event:/sub/cyclops/install_mod");//sound that plays when item is activated
        public FMODAsset DeactivateSound = UtilityStuffs.Utility.GetFmodAsset("event:/tools/battery_die");//sound that plays when item is deactivated
        public FMODAsset ActivateFailSound = UtilityStuffs.Utility.GetFmodAsset("event:/tools/transfuser/fail");//sound that plays when item can't be activated

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

        internal void UpdateEquipped()
        {
            if (AutomaticSetup)
            {
                var temp = UtilityStuffs.Utility.EquipmentHasItem(techType, equipmentType);
                if (temp != equipped)
                {
                    if (temp && OnEquip != null) OnEquip.Invoke();
                    else if(!temp && OnUnEquip != null) OnUnEquip.Invoke();
                }
                equipped = temp;

                if (InvertIcon)
                {
                    if(container != null && container.transform != null) {
                        container.transform.eulerAngles = new Vector3(0, 180, 180);//for some reason the angle would be off unless I set it here
                    }
                    else
                    {
                        //I still want to know about this, but it also is run every time when the game quits so I only want to know outside of the game being quit
                        //Logger.Log(Logger.Level.Warn, $"icon Container null: {container == null}, {(container == null ? "" : $"Transform null: {container.transform != null}, " )} If you get this message, ping Nagorrogan in the subnautica modding discord and send the log file to me");
                    }
                }
            }
        }
        internal void Update()
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

            if (Input.GetKeyDown(activateKey) && (CanActivate != null? CanActivate.Invoke() : CanActivateDefault()))
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
            else if(Input.GetKeyDown(activateKey))
            {
                if (QMod.config.SoundsActive && playSounds && ActivateFailSound)
                {
                    Logger.Log(Logger.Level.Info, "Playing fail sound", null, true);
                    Utils.PlayFMODAsset(ActivateFailSound);
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
            if(fadeBackground)
                this.itemIcon.foreground.material.SetFloat(ShaderPropertyID._FillValue, (100f / (this.MaxCharge / this.charge)) - 50f);
        }
        private void HandleActivation()
        {
            if (OnceOff && charge < DrainRate)
            {
                if (QMod.config.SoundsActive && playSounds && ActivateFailSound)
                {
                    Utils.PlayFMODAsset(ActivateFailSound);
                }
                return;
            }
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
                Utils.PlayFMODAsset(ActivateSound, Player.main.transform);
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
                Utils.PlayFMODAsset(DeactivateSound, Player.main.transform);
            }
        }
        private bool CanActivateDefault()
        {
            Player player = Player.main;
            return player != null && !player.isPiloting && player.mode == Player.Mode.Normal && !player.GetPDA().isOpen;
        }
    }
}
