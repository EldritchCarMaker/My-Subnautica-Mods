using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace EquippableItemIcons.API
{
    public class ActivatedEquippableItem : HudItemIcon
    {
        public ActivatedEquippableItem(string name, Atlas.Sprite sprite, TechType itemTechType) : base(name, sprite, itemTechType)
        {

        }
        public bool active = false;//whether the item is currently being used, not the same as equipped or icon active. Also avoid touching if using AutomaticSetup

        public ToggleEvent Activate;//the actual functionality of item, when its activated what does it do?
        public ToggleEvent Deactivate;//stop the functionality of item

        public FMODAsset ActivateSound = UtilityStuffs.Utility.GetFmodAsset("event:/sub/cyclops/install_mod");//sound that plays when item is activated
        public FMODAsset DeactivateSound = UtilityStuffs.Utility.GetFmodAsset("event:/tools/battery_die");//sound that plays when item is deactivated
        public FMODAsset ActivateFailSound = UtilityStuffs.Utility.GetFmodAsset("event:/tools/transfuser/fail");//sound that plays when item can't be activated

        public bool playSounds = true;//whether this item plays sounds or not, overriden by the mod config

        public AllowedEvent CanActivate;//used to tell if the item can currently be activated or not. Has a default, but good for extra conditions specific to the item

        public delegate bool DetailedAllowedEvent(List<TechType> techTypes);
        public DetailedAllowedEvent DetailedCanActivate;//same as normal can activate, but this one gives you the techtypes equipped

        public delegate void DetailedEvent(List<TechType> techTypes);
        public DetailedEvent DetailedActivate;

        public float MaxCharge = 100;
        public float MinCharge = 0;
        public float ChargeRate = 20;
        public float DrainRate = 5;
        public float charge;
        public float RechargeDelay = 0;


        public float MaxIconFill = 50;//hard to explain. When icon fades it goes from this value up from the center down to the next value down from the center
        //mid point of fade is always 0
        //top point of fade will be this value
        //try to keep it as close to filling the icon as possible, otherwise it may look like the icon isn't changing despite the charge draining
        public float MinIconFill = -50;
        //bottom point of fade will be this value. Generally fine to keep here, this tends to reach bottom of screen anyway


        public KeyCode activateKey = KeyCode.None;

        public enum ActivationType
        {
            Toggle,
            OnceOff,
            Held
        }
        public ActivationType activationType = ActivationType.Toggle;

        private float TimeCharge = 0;

        public new void Update()
        {
            if (!equipped)
            {
                if (active)
                {
                    HandleDeactivation();
                }
                return;
            }

            if (activationType == ActivationType.Held)
            {
                if (Input.GetKey(activateKey))
                {
                    if (!active && (CanActivate != null ? CanActivate.Invoke() : CanActivateDefault()))
                        HandleActivation();
                }
                else
                {
                    if (active)
                        HandleDeactivation();
                }
            }
            else
            {
                bool KeyDown = Input.GetKeyDown(activateKey);
                if (KeyDown && (DetailedCanActivate != null ? DetailedCanActivate.Invoke(equippedTechTypes) : CanActivate != null ? CanActivate.Invoke() : CanActivateDefault()))
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
                else if (KeyDown)
                {
                    if (QMod.config.SoundsActive && playSounds && ActivateFailSound)
                    {
                        Utils.PlayFMODAsset(ActivateFailSound);
                    }
                }
            }


            if (activationType == ActivationType.OnceOff)
            {
                if (Time.time >= TimeCharge)
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

            float chargePercent = 100f / (this.MaxCharge / this.charge);
            float fillValue = Mathf.Lerp(MinIconFill, MaxIconFill, chargePercent / 100);

            this.itemIcon.foreground.material.SetFloat(ShaderPropertyID._FillValue, fillValue);
            //percent minus 50 because for some reason this value is offset. 50 is max, -50 is minimum. 
            if (fadeBackground)
                this.itemIcon.background.material.SetFloat(ShaderPropertyID._FillValue, fillValue);
        }
        private void HandleActivation()
        {
            if (activationType == ActivationType.OnceOff && charge < DrainRate)
            {
                if (QMod.config.SoundsActive && playSounds && ActivateFailSound)
                {
                    Utils.PlayFMODAsset(ActivateFailSound);
                }
                return;
            }

            if (DetailedActivate != null) 
                DetailedActivate.Invoke(equippedTechTypes);
            else
                Activate?.Invoke();
            
            if (activationType == ActivationType.OnceOff)
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
            if (activationType == ActivationType.OnceOff)
            {
                //I'm sure I'll add stuff here later

                //and I never did
            }
            else
            {
                active = false;
            }
            if (QMod.config.SoundsActive && playSounds && DeactivateSound)
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