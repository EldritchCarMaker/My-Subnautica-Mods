using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static CyclopsVehicleUpgradeConsole.VehicleConsoleCreation;
#if !SN2
using SMLHelper.V2.Utility;
#else
using Nautilus.Utility;
#endif

namespace CyclopsVehicleUpgradeConsole.Monobehaviours
{
    public class SwapButton : HandTarget, IHandTarget, IEventSystemHandler, IPointerClickHandler, IPointerHoverHandler
    {
        private const string hoverText = "Swap Screens";
        public bool colorScreenActive = false;
        readonly string AssetsFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");
        public void OnHandClick(GUIHand hand)
        {
            if (colorScreenActive)
            {
                colorScreenActive = false;
                SetActive(gameObject);
            }
            else
            {
                colorScreenActive = true;
                SetInActive(gameObject);
            }
        }
        public override void Awake()
        {
            base.Awake();

            Atlas.Sprite myAtlas = ImageUtils.LoadSpriteFromFile(Path.Combine(AssetsFolder, "PageChangerBackground.png"));
            var texture = myAtlas.texture;
            var sprite = UnityEngine.Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), Vector2.one * 0.5f);
            gameObject.transform.GetChild(0).GetComponent<Image>().sprite = sprite;
        }
        public void OnHandHover(GUIHand hand)
        {
            HandReticle.main.SetIcon(HandReticle.IconType.Hand, 1f);
#if SN1
            HandReticle.main.SetInteractText(hoverText);
#else
            HandReticle.main.SetTextRaw(HandReticle.TextType.Hand, hoverText);
#endif
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnHandClick(null);
        }

        public void OnPointerHover(PointerEventData eventData)
        {
            OnHandHover(null);
        }
    }
}
