using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CameraDroneRepairUpgrade
{
    public class uGUI_RepairUpgradeIcon : MonoBehaviour
    {
        public uGUI_RepairUpgradeIcon()
        {
        }

        public static uGUI_RepairUpgradeIcon main;
        public Vector2 iconSize = new Vector2(108f, 108f);
        public float timeIn = 1f;
        public float timeOut = 0.5f;
        public float oscReduction = 100f;
        public float oscFrequency = 5f;
        public float oscScale = 2f;
        public float oscDuration = 2f;
        public uGUI_ItemIcon icon;
        private Sequence sequence = new Sequence();
        private bool show;
        private float oscSeed;
        private float oscTime;

        private void Awake()
        {
            if (main == null)
            {
                main = this;
                GameObject go = new GameObject("RepairToolIcon");
                go.transform.SetParent(gameObject.transform);
                icon = go.AddComponent<uGUI_ItemIcon>();
                icon.Init(null, transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
                icon.SetForegroundSprite(SpriteManager.Get(SpriteManager.Group.Item, TechType.Welder.AsString()));
                icon.SetBackgroundSprite(SpriteManager.GetBackground(CraftData.BackgroundType.PlantAirSeed));
                icon.SetSize(iconSize);
                icon.SetBackgroundRadius(Mathf.Min(iconSize.x, iconSize.y) * 0.5f);
                Color color = new Color(1f, 0.6f, 0f, 2f);
                icon.SetBackgroundColors(color, color, color);
                SetAlpha(0f);
                sequence.ForceState(false);
                return;
            }
            Destroy(this);
        }

        private void LateUpdate()
        {
            if (sequence.target != show)
            {
                if (show && sequence.t == 0f)
                {
                    oscTime = Time.time;
                    oscSeed = UnityEngine.Random.value;
                }
                sequence.Set(show ? timeIn : timeOut, show);
            }
            if (sequence.active)
            {
                if (sequence.t > 0f)
                {
                    float num = 0f;
                    float num2 = 0f;
                    float t = Mathf.Clamp01((Time.time - oscTime) / oscDuration);
                    MathExtensions.Oscillation(oscReduction, oscFrequency, oscSeed, t, out num, out num2);
                    icon.rectTransform.localScale = new Vector3(1f + num * oscScale, 1f + num2 * oscScale, 1f);
                }
                sequence.Update();
            }
            SetAlpha(sequence.target ? 1f : sequence.t);
            show = false;
        }

        public void SetAlpha(float alpha)
        {
            icon.SetAlpha(alpha, alpha, alpha);
        }

        public void Show()
        {
            show = true;
        }
    }
}
