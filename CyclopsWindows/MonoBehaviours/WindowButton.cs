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
        private SubRootMarker marker;
        public void Awake()
        {
            marker = GetComponentInParent<SubRootMarker>();
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            marker.FlipLockers();
        }
    }
}
