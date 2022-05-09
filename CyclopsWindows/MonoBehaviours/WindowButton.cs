using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CyclopsWindows.MonoBehaviours
{
    internal class WindowButton : MonoBehaviour, IPointerClickHandler
    {
        public void OnPointerClick(PointerEventData eventData)
        {
            GetComponentInParent<SubRootMarker>().FlipLockers();
        }
    }
}
