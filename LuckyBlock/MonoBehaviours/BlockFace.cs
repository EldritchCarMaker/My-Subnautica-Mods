using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LuckyBlock.MonoBehaviours
{
    internal class BlockFace : MonoBehaviour
    {
        [SerializeField]
        private GameObject _precursorModel;
        [SerializeField]
        private GameObject _normalModel;

        private bool _isPrecursor = false;

        public bool IsPrecursor
        {
            get { return _isPrecursor; }
            set
            {
                _isPrecursor = value;
                _normalModel.SetActive(!value);
                _precursorModel.SetActive(value);
            }
        }
    }
}
