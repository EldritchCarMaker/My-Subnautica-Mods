using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ArmorSuit.Mono
{
    public class SuitColors : MonoBehaviour//thanks again lee
    {
        public Renderer[] renderers;

        private Color _lastAppliedColor = Color.black;
        private Color _currentColor = Color.black;
        private Gradient _gradient;
        private float _timeColorLastChanged = 0;

        private float _transitionDuration = 0.5f;

        private void Update()
        {
            // update color based on gradient, if possible
            if (_timeColorLastChanged > 0f)
            {
                _currentColor = _gradient.Evaluate(Mathf.Clamp01((Time.time - _timeColorLastChanged) / _transitionDuration));
            }
            // update materials
            if (!_currentColor.Equals(_lastAppliedColor))
            {
                foreach (var renderer in renderers)
                {
                    foreach (var material in renderer.materials)
                    {
                        material.color = _currentColor;
                    }
                }
                _lastAppliedColor = _currentColor;
            }
        }

        public void SetColor(Color newColor)
        {
            _timeColorLastChanged = Time.time;
            _gradient = new Gradient() { colorKeys = new GradientColorKey[] { new GradientColorKey(_currentColor, 0f), new GradientColorKey(newColor, 1f) } };
        }
    }
}
