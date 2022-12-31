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
#if SN
            Pickupable.overrideTechType = TechType.Cyclops;
            Pickupable.overrideTechUsed = true;
#else
            Pickupable.overrideTechType = CraftData.GetTechType(gameObject);
            Pickupable.overrideTechUsed = true;
#endif
            base.Awake();
        }
        public void OnHandClick(GUIHand hand)
        {
#if SN
            if (Player.main.currentSub != null) return;
#else
            if (Player.main.currentInterior != null) return;//why didn't they bring this to 2.0? This is nice system
#endif

            if(GameInput.GetButtonHeld(GameInput.Button.Sprint))
            {
#if BZ
                if(TryGetComponent<SeaTruckSegment>(out var segment))
                {
                    segment.GetFirstSegment().GetComponent<Pickupable>().OnHandClick(hand);
                    return;
                }
#endif
                Pickupable.OnHandClick(hand);
            }
        }
        public void OnHandHover(GUIHand hand)
        {
#if SN
            if (Player.main.currentSub != null) return;
#else
            if (Player.main.currentInterior != null) return;//why didn't they bring this to 2.0? This is nice system
#endif

            if (GameInput.GetButtonHeld(GameInput.Button.Sprint))
            {
#if BZ
                if (TryGetComponent<SeaTruckSegment>(out var segment))
                {
                    segment.GetFirstSegment().GetComponent<Pickupable>().OnHandHover(hand);
                    return;
                }
#endif
                Pickupable.OnHandHover(hand);
            }
        }
    }
}
