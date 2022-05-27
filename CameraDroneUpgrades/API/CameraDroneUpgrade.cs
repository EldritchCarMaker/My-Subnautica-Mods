using SMLHelper.V2.Assets;
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
        internal CameraDroneUpgrade(string name, Craftable item, Action setupMethod)
        {
            this.name = name;
            this.item = item;
            techType = item.TechType;
            setUp += setupMethod;
        }
        public enum ActivationType
        {
            Held,
            OnceOff
        }
        public ActivationType activationType = ActivationType.OnceOff;
        public KeyCode key = KeyCode.F;
        public Action activate;
        public Action deactivate;
        public Action update;
        public Action equippedUpdate; //only called when equipped, as opposed to normal update which is always called
        public Action setUp;
        public Craftable item;
        public string name;

        public MapRoomCamera camera { get; internal set; }
        public TechType techType { get; internal set; }
        public bool active { get; internal set; }
    }
}
