using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace EldritchMoth.Mono
{
    internal class EldritchMothMono : MonoBehaviour
    {
        private const int MaxSpeedSetting = 3;
        private const float SpeedMultiplier = 1.5f;

        private SeaMoth _seaMoth;
        private int _currentSpeedSetting;

        private enum SpeedSetting
        {
            Slowest,
            Slower,
            Slow,
            Normal,
            Fast,
            Faster,
            Fastest
        }

        public void Awake()
        {
            _seaMoth = GetComponent<SeaMoth>();
        }

        public void Update()
        {
            _seaMoth.crushDamage.crushDepth = 69420;//nice
        }

        public void SpeedUp()
        {
            if(_currentSpeedSetting >= MaxSpeedSetting)
            {
                ErrorMessage.AddMessage("Max Speed, can't go higher!");
                return;
            }
            _currentSpeedSetting++;
            _seaMoth.forwardForce *= SpeedMultiplier;
            _seaMoth.backwardForce *= SpeedMultiplier;
            _seaMoth.sidewardForce *= SpeedMultiplier;
            _seaMoth.verticalForce *= SpeedMultiplier;
            OnSpeedChanged();
        }

        public void SpeedDown()
        {
            if (_currentSpeedSetting <= -MaxSpeedSetting)
            {
                ErrorMessage.AddMessage("Min Speed, can't go lower!");
                return;
            }
            _currentSpeedSetting--;
            _seaMoth.forwardForce /= SpeedMultiplier;
            _seaMoth.backwardForce /= SpeedMultiplier;
            _seaMoth.sidewardForce /= SpeedMultiplier;
            _seaMoth.verticalForce /= SpeedMultiplier;
            OnSpeedChanged();
        }

        public void OnSpeedChanged()
        {
            ErrorMessage.AddMessage($"Speed now {(SpeedSetting)(_currentSpeedSetting + MaxSpeedSetting)}");
        }
    }
}
