using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CameraDroneUpgrades.API
{
    public class CameraDroneUpgrade
    {
        internal CameraDroneUpgrade(string name, TechType type, Action setupMethod)
        {
            this.name = name;//name of upgrade

            techType = type;//techtype of the upgrade
            
            setUp += setupMethod;//what method to call when setting up upgrade
        }
        public enum ActivationType
        {
            Held,//button must be held to be used
            OnceOff//button is pressed to be used
        }
        public ActivationType activationType = ActivationType.OnceOff;//activation type out of the two listed above

        public KeyCode key = KeyCode.F;//what key activates the upgrade

        public Action activate;//event for when upgrade is activated by user

        public Action deactivate;//event for when upgrade is deactivated
        //for held upgrades, this is when button is released
        //for once off, this is when button is pressed a second time
        //if you want an upgrade to do something on both activate and deactivate, simply assign both events to the same method

        public Action unEquip;//called when camera is exited, camera must be set back as if upgrade was never used

        public Action update;//called every frame. Used so that you don't have to make your upgrade into a monobehaviour

        public Action equippedUpdate;//same as above, but only called if the upgrade techtype is currently equipped

        public Action setUp;//as mentioned above

        public string name;//as mentioned above

        public MapRoomCamera camera { get; internal set; }//the last used camera. Will not always be used by the player, but should always be used when the above events are invoked
        public TechType techType { get; internal set; }//as explained above
        public bool active { get; internal set; }//whether the upgrade is currently active or not
    }
}
