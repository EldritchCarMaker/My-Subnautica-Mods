using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace EquippableItemIcons.API
{
    public class ActivatedVehicleItem : HudItemIcon
    {
        public ActivatedVehicleItem(string name, Atlas.Sprite sprite, TechType itemTechType) : base(name, sprite, itemTechType)
        {
            //for vehicle modules that want an icon 
        }
        public bool active = false;//whether the item is currently being used, not the same as equipped or icon active. Also avoid touching if using AutomaticSetup

        public delegate void VehicleToggleEvent(Vehicle vehicle);

        public VehicleToggleEvent Activate;//the actual functionality of item, when its activated what does it do?
        public VehicleToggleEvent Deactivate;//stop the functionality of item

        public FMODAsset ActivateSound = UtilityStuffs.Utility.GetFmodAsset("event:/sub/cyclops/install_mod");//sound that plays when item is activated
        public FMODAsset DeactivateSound = UtilityStuffs.Utility.GetFmodAsset("event:/tools/battery_die");//sound that plays when item is deactivated
        public FMODAsset ActivateFailSound = UtilityStuffs.Utility.GetFmodAsset("event:/tools/transfuser/fail");//sound that plays when item can't be activated

        public bool playSounds = true;//whether this item plays sounds or not, overriden by the mod config

        public delegate bool VehicleAllowedEvent(Vehicle vehicle);
        public AllowedEvent CanActivate;//used to tell if the item can currently be activated or not. Has a default, but good for extra conditions specific to the item

        public delegate bool DetailedVehicleAllowedEvent(Vehicle vehicle, List<TechType> techTypes);
        public DetailedVehicleAllowedEvent DetailedCanActivate;//same as normal can activate, but this one gives you the techtypes equipped

        public delegate void DetailedVehicleEvent(Vehicle vehicle, List<TechType> techTypes);
        public DetailedVehicleEvent DetailedActivate;

        public float MaxCharge = 100;
        public float MinCharge = 0;
        public float ChargeRate = 20;
        public float DrainRate = 5;
        public float charge;
        public float RechargeDelay = 0;

        public Vehicle currVehicle => Player.main.currentMountedVehicle;

        public float MaxIconFill = 50;//hard to explain. When icon fades it goes from this value up from the center down to the next value down from the center
        //mid point of fade is always 0
        //top point of fade will be this value
        //try to keep it as close to filling the icon as possible, otherwise it may look like the icon isn't changing despite the charge draining
        public float MinIconFill = -50;
        //bottom point of fade will be this value. Generally fine to keep here, this tends to reach bottom of screen anyway

        public enum ActivationType
        {
            Toggle,
            OnceOff
        }
        public ActivationType activationType = ActivationType.Toggle;

        private float TimeCharge = 0;//for recharge delay
        internal static void OnModuleUse()
        {

        }
        internal override void Update()
        {
            base.Update();

            if (Player.main.currentMountedVehicle == null) return;

            UpdateEquipped();

            if (!equipped)
            {
                if (active)
                {
                    HandleDeactivation();
                }
                return;
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

            if (AutoIconFade)
                UpdateFill();
        }
        internal override void UpdateEquipped()
        {
            if (!AutomaticSetup || currVehicle == null)
            {
                return;
            }

            equippedTechTypes.Clear();

            var temp = currVehicle.modules.GetCount(techType) > 0;

            if (temp) equippedTechTypes.Add(techType);

            if (SecondaryTechTypes != null)
            {
                foreach (TechType type in SecondaryTechTypes)
                {
                    if (currVehicle.modules.GetCount(type) > 0)
                    {
                        temp = true;
                        equippedTechTypes.Add(type);
                    }
                }
            }

            if (InvertIcon)
            {
                if (container != null && container.transform != null)
                {
                    container.transform.eulerAngles = new Vector3(0, 180, 180);//for some reason the angle would be off unless I set it here
                }
                else
                {
                    //I still want to know about this, but it also is run every time when the game quits so I only want to know outside of the game being quit
                    //Logger.Log(Logger.Level.Warn, $"icon Container null: {container == null}, {(container == null ? "" : $"Transform null: {container.transform != null}, " )} If you get this message, ping Nagorrogan in the subnautica modding discord and send the log file to me");
                }
            }

            if (IsEquipped != null)
            {
                temp = IsEquipped.Invoke();
            }
            equipped = temp;
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
        internal virtual void HandleActivation()
        {
            if(currVehicle == null)
            {
                ErrorMessage.AddMessage("Activate with null Vehicle!");
                return;
            }
            ErrorMessage.AddMessage("Activated");
            if (activationType == ActivationType.OnceOff && charge < DrainRate)
            {
                if (QMod.config.SoundsActive && playSounds && ActivateFailSound)
                {
                    Utils.PlayFMODAsset(ActivateFailSound);
                }
                return;
            }

            if (DetailedActivate != null)
                DetailedActivate.Invoke(currVehicle, equippedTechTypes);
            else
                Activate?.Invoke(currVehicle);

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
        internal virtual void HandleDeactivation()
        {
            Deactivate?.Invoke(currVehicle);
            if (activationType == ActivationType.OnceOff)
            {
                //I'm sure I'll add stuff here later
                //...

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
        public virtual bool CanActivateDefault()
        {
            Player player = Player.main;
            return player != null && player.isPiloting && currVehicle != null && !player.GetPDA().isOpen;
        }
    }
}
