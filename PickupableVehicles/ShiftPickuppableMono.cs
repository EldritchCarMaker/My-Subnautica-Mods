using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PickupableVehicles
{
    internal class ShiftPickuppableMono : HandTarget, IHandTarget
    {
        public Pickupable Pickupable { get; set; }

        public override void Awake()
        {
            Pickupable = gameObject.EnsureComponent<Pickupable>();
            Pickupable.overrideTechType = TechType.Cyclops;
            Pickupable.overrideTechUsed = true;
            base.Awake();
        }
        public void OnHandClick(GUIHand hand)
        {
            if (Player.main.currentSub != null) return;

            if(GameInput.GetButtonHeld(GameInput.Button.Sprint))
            {
                Pickupable.OnHandClick(hand);
            }
        }
        public void OnHandHover(GUIHand hand)
        {
            if (Player.main.currentSub != null) return;

            if (GameInput.GetButtonHeld(GameInput.Button.Sprint))
            {
                Pickupable.OnHandHover(hand);
            }
        }
    }
}
