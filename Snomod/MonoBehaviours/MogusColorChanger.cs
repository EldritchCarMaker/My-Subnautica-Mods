using System.Collections.Generic;
using UnityEngine;

namespace Snomod.MonoBehaviours
{
    public class MogusColorChanger : MonoBehaviour
    {
        public enum ColorType
        {
            Red,
            Blue,
            Green,
            Pink,
            Orange,
            Yellow,
            Black,
            White,
            Purple,
            Brown,
            Cyan,
            Lime,
            None
        }
        public static Vector3[] RGBs { get; } = new[] 
        { 
            new Vector3(197, 17, 17), 
            new Vector3(19, 46, 209), 
            new Vector3(17, 127, 45), 
            new Vector3(237, 84, 186), 
            new Vector3(239, 125, 13), 
            new Vector3(245, 245, 87),
            new Vector3(63, 71, 78),
            new Vector3(214, 224, 240),
            new Vector3(107, 47, 187),
            new Vector3(113, 73, 30),
            new Vector3(56, 254, 220),
            new Vector3(80, 239, 57)
        };
        public static Color GetColor(ColorType type)
        {
            var rgb = RGBs[(int)type];
            return new Color(rgb.x / 255f, rgb.y / 255f, rgb.z / 255f);
        }
        private ColorType _color = ColorType.None;
        public ColorType Color { get => _color; set { UpdateColor(value); _color = value; } }

        public Renderer[] renderers;
        public Renderer renderer => renderers[0];
        public void Awake()
        {
            renderers.ForEach((renderer) => { renderer.materials.ForEach(mat => mat.shader = Shader.Find("MarmosetUBER")); });

            if (Color == ColorType.None) 
                Color = (ColorType)Random.Range(0, RGBs.Length);
        }
        private void UpdateColor(ColorType color) => UpdateColor(GetColor(color));
        private void UpdateColor(Color color)
        {
            renderer.material.color = color;
        }
    }
}
