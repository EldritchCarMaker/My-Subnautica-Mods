using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PickupableVehicles
{
    internal class ShiftPickuppableMono : HandTarget, IHandTarget
    {
        private Pickupable _pickupable;
        public Pickupable Pickupable 
        { 
            get 
            {
                if(!_pickupable) _pickupable = gameObject.AddComponent<Pickupable>();
                return _pickupable;
            } 
        }
        public TechType overrideTech = TechType.Cyclops;
        public void Start()
        {
            if (TryGetComponent<Pickupable>(out var pick)) GameObject.DestroyImmediate(pick);
#if SN
            Pickupable.overrideTechType = overrideTech;
            Pickupable.overrideTechUsed = true;
#else
            Pickupable.overrideTechType = CraftData.GetTechType(gameObject);
            Pickupable.overrideTechUsed = true;
#endif
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
                    segment.GetFirstSegment().gameObject.EnsureComponent<Pickupable>().OnHandHover(hand);
                    return;
                }
#endif
                Pickupable.OnHandHover(hand);
            }
        }
    }
}
