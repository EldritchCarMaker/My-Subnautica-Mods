using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace EquippableItemIcons.API
{
    public class ChargableEquippableItem : HudItemIcon
    {
        public const float SoundCooldown = 3f;
        public float timeNextSound = 0;

        public ChargableEquippableItem(string name, Atlas.Sprite sprite, TechType itemTechType) : base(name, sprite, itemTechType)
        {
            //for items that have an ability that can be charged up before being used
        }
        public bool charging = false;//whether the item is currently being charged, not the same as equipped or icon active. Also avoid touching if using AutomaticSetup

        public ToggleEvent StartCharging;//event for when item begins charging
        public ToggleEvent ReleasedCharging;//event for when item is released

        public FMODAsset ChargingStartSound = UtilityStuffs.Utility.GetFmodAsset("event:/sub/cyclops/install_mod");//sound that plays when item begins charging
        public FMODAsset ChargingReleasedSound = UtilityStuffs.Utility.GetFmodAsset("event:/tools/battery_die");//sound that plays when item is released
        public FMODAsset CantChargeSound = UtilityStuffs.Utility.GetFmodAsset("event:/tools/transfuser/fail");//sound that plays when item can't be charged

        public bool playSounds = true;//whether this item plays sounds or not, overriden by the mod config

        public AllowedEvent CanActivate;//used to tell if the item can currently be activated or not. Has a default, but good for extra conditions specific to the item

        public delegate bool DetailedAllowedEvent(List<TechType> techTypes);
        public DetailedAllowedEvent DetailedCanActivate;//same as normal can activate, but this one gives you the techtypes equipped

        public delegate void DetailedEvent(List<TechType> techTypes);
        public DetailedEvent DetailedActivate;

        public float MaxCharge = 100;
        public float MinCharge = 0;
        public float ChargeRate = 20;
        public float MinChargeRequiredToTrigger = 0;
        public float charge;
        public bool AutoReleaseOnMaxCharge = false;


        public float MaxIconFill = 50;//hard to explain. When icon fades it goes from this value up from the center down to the next value down from the center
        //mid point of fade is always 0
        //top point of fade will be this value
        //try to keep it as close to filling the icon as possible, otherwise it may look like the icon isn't changing despite the charge draining
        public float MinIconFill = -50;
        //bottom point of fade will be this value. Generally fine to keep here, this tends to reach bottom of screen anyway


        public KeyCode activateKey = KeyCode.None;

        internal override void Update()
        {
            //iconActive = IsIconActive != null ? IsIconActive.Invoke() : equipped;
            base.Update();

            if (!equipped || !AutomaticSetup)
                return;

            bool keyPressed = Input.GetKey(activateKey);

            bool canActivate = DetailedCanActivate != null ? DetailedCanActivate.Invoke(equippedTechTypes) : CanActivate != null ? CanActivate.Invoke() : CanActivateDefault();

            bool shouldPlaySound = CanActivateDefault();//sound was annoying when it was played with pda, seamoth, etc

            if (keyPressed && canActivate)
            {
                HandleActivation();
            }
            else if (keyPressed)
            {
                if (QMod.config.SoundsActive && playSounds && CantChargeSound && shouldPlaySound)
                {
                    if (Time.time >= timeNextSound)
                    {
                        timeNextSound = Time.time + SoundCooldown;
                        Utils.PlayFMODAsset(CantChargeSound);
                    }
                }
            }
            else if (charge != MinCharge && charge >= MinChargeRequiredToTrigger && Time.time >= timeNextSound)
                HandleDeactivation();
            else charge = 0;
            if(AutoIconFade)
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
            if (charge == MinCharge)
                StartCharging?.Invoke();

            if (charge != MaxCharge)
                charge = Mathf.Min(charge + (ChargeRate * Time.deltaTime), MaxCharge);

            if (charge == MaxCharge && AutoReleaseOnMaxCharge)
                HandleDeactivation();
        }
        private void HandleDeactivation()
        {
            ReleasedCharging?.Invoke();
            charge = MinCharge;
            timeNextSound = Time.time + SoundCooldown;

            if (QMod.config.SoundsActive && playSounds && ChargingReleasedSound)
            {
                Utils.PlayFMODAsset(ChargingReleasedSound, Player.main.transform);
            }
        }

        public bool CanActivateDefault()
        {
            Player player = Player.main;
            return player != null && !player.isPiloting && player.mode == Player.Mode.Normal && !player.GetPDA().isOpen;
        }
    }
}
