using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CyclopsAntennaFix
{
    internal class AntennaPositionFixer : MonoBehaviour
    {
        public Transform antennaTransform;
        public static readonly Vector3 originalLocalPosition = new Vector3(73.75f, 0.38f, -14.24f);
        public void Awake()
        {
            antennaTransform = transform.Find("CyclopsMeshAnimated/cyclops_antennae");
            if (antennaTransform == null) Destroy(this);
        }
        public void Update()
        {
            antennaTransform.localPosition = originalLocalPosition;
        }
    }
}
