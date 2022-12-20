using ArmorSuit.Mono;
using EquippableItemIcons.API;
using HarmonyLib;
#if SN1
using Oculus.Newtonsoft.Json;
#else
using Newtonsoft.Json;
#endif
using SMLHelper.V2.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UWE;
using Valve.VR;
using static OVRPlugin;
using Logger = QModManager.Utility.Logger;

namespace ArmorSuit
{
    public class ArmorSuitMono : MonoBehaviour, IOnTakeDamage
    {
#region Item Icon Stuff
        public static string AssetsFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");

        private static Atlas.Sprite _armorSuitSprite = ImageUtils.LoadSpriteFromFile(Path.Combine(AssetsFolder, "ArmorSuitIcon.png"));
        private static Atlas.Sprite _armorSuitSpriteGloveless = ImageUtils.LoadSpriteFromFile(Path.Combine(AssetsFolder, "armorsuitIcon-gloveless.png"));

        public ActivatedEquippableItem hudItemIcon = new ActivatedEquippableItem("ArmorSuitIcon", _armorSuitSprite, ArmorSuitItem.thisTechType);
#endregion

        /*internal static readonly List<DamageType> UnaffectedTypes = new List<DamageType>()
        {
            commented out in case someone wishes to edit the config to include one of these types
            I won't be using any, but maybe someone *really* wants to take less damage from smoke or something
            DamageType.Undefined,
            DamageType.Pressure,
            DamageType.LaserCutter,
            DamageType.Starve,
            DamageType.Smoke,
        };*/

        private static GameObject _diveSuitGloves;
        private static GameObject _diveSuitBody;

        private static GameObject _reinforcedSuitGloves;
        private static GameObject _reinforcedSuitBody;

        private static Texture _vanillaRSuitBodyTexture;
        private static Texture _vanillaRSuitArmsTexture;

        private static Texture _newRSuitBodyTexture;
        private static Texture _newRSuitArmsTexture;

        private static List<DefenseInfo> defenseInfos => QMod.config.DefenseInfos;

        private DefenseInfo _currentType;
        private ArmorSuitDefense SuitDefense { get; } = new ArmorSuitDefense();
        public DefenseInfo CurrentType
        {
            get 
            {
                if (_currentType == null || _currentType.DamageTypes == null) _currentType = defenseInfos[0];
                return _currentType; 
            }

            set
            {
                if (CurrentType == value) return;
                    
                _currentType = value;
                ErrorMessage.AddMessage("Defense type now " + CurrentType.Type);
                UpdateDamageModifier();
                SetColors(CurrentType.SuitColor);
            }
        }
        private SuitColors SuitColors;

        public void Awake()
        {
            SuitDefense.SetUpDamageModifiers();
            FindBodyObjects();
            SetUpSuitColors();

            hudItemIcon.activationType = ActivatedEquippableItem.ActivationType.OnceOff;
            hudItemIcon.activateKey = QMod.config.ArmorSuitKey;
            hudItemIcon.AutoIconFade = false;
            hudItemIcon.DrainRate = 0;
            hudItemIcon.equipmentType = EquipmentType.Body;
            hudItemIcon.techType = ArmorSuitItem.thisTechType;
            hudItemIcon.itemTechTypes.Add(ArmorGlovesItem.techType, EquipmentType.Gloves);

            hudItemIcon.DetailedActivate += Activate;
            hudItemIcon.OnEquipChange += EquipChange;

            Registries.RegisterHudItemIcon(hudItemIcon);
        }

        private static void FindBodyObjects()
        {
            var geo = Player.main.transform.Find("body/player_view/male_geo");

            var diveSuit = geo.Find("diveSuit");

            if (!_diveSuitGloves)
            {
                _diveSuitGloves = diveSuit.Find("diveSuit_hands_geo").gameObject;
            }
            if(!_diveSuitBody)
            {
                _diveSuitBody = diveSuit.Find("diveSuit_body_geo").gameObject;
            }

            var suit = geo.Find("reinforcedSuit");

            if(!_reinforcedSuitGloves)
            {
                _reinforcedSuitGloves = suit.Find("reinforced_suit_01_glove_geo").gameObject;
            }
            if(!_reinforcedSuitBody)
            {
                _reinforcedSuitBody = suit.Find("reinforced_suit_01_body_geo").gameObject;
            }

            if(!_vanillaRSuitArmsTexture)
            {
                _vanillaRSuitArmsTexture = _reinforcedSuitGloves.GetComponent<Renderer>().material.mainTexture;
            }
            if(!_vanillaRSuitBodyTexture)
            {
                _vanillaRSuitBodyTexture = _reinforcedSuitBody.GetComponent<Renderer>().material.mainTexture;
            }

            if(!_newRSuitArmsTexture)
            {
                _newRSuitArmsTexture = ImageUtils.LoadTextureFromFile(Path.Combine(AssetsFolder, "ColoredSuitArms.png"));
            }
            if(!_newRSuitBodyTexture)
            {
                _newRSuitBodyTexture = ImageUtils.LoadTextureFromFile(Path.Combine(AssetsFolder, "ColoredSuitBody.png"));
            }
        }
        private void SetUpSuitColors()
        {
            SuitColors = gameObject.EnsureComponent<SuitColors>();
            SuitColors.renderers = _reinforcedSuitGloves.transform.parent.GetComponentsInChildren<Renderer>(true);
        }

        public void Activate(List<TechType> equippedTypes)
        {
            bool foundCurrent = false;
            foreach(var info in defenseInfos)
            {
                if(foundCurrent)
                {
                    CurrentType = info;
                    foundCurrent = false;
                    break;
                }

                if (info == CurrentType) foundCurrent = true;
            }
            if(foundCurrent)//if it found the current type, but it never moved onto the next type in the sequence. (ie; it reached the last type in the list)
            {
                CurrentType = defenseInfos[0];
            }
        }

        public void EquipChange()
        {
            UpdateDamageModifier();
            UpdateIcon();
            UpdatePlayerSuit();
        }

        public void UpdatePlayerSuit()
        {
            _diveSuitBody.transform.parent.gameObject.SetActive(true);
            _reinforcedSuitBody.transform.parent.gameObject.SetActive(true);//should be active anyway, but can't hurt to make sure


            var notEquipped = hudItemIcon.equippedTechTypes.Count == 0;

            var color = notEquipped ? Color.white : CurrentType.SuitColor;
            SuitColors.SetColor(color);


            var glovesEquipped = hudItemIcon.equippedTechTypes.Contains(ArmorGlovesItem.techType);
            _reinforcedSuitGloves.SetActive(glovesEquipped);
            _diveSuitGloves.SetActive(!glovesEquipped);

            _reinforcedSuitGloves.GetComponent<Renderer>().material.mainTexture = glovesEquipped ? _newRSuitArmsTexture : _vanillaRSuitArmsTexture;


            var suitEquipped = hudItemIcon.equippedTechTypes.Contains(hudItemIcon.techType);
            _reinforcedSuitBody.SetActive(suitEquipped);
            _diveSuitBody.SetActive(!suitEquipped);

            _reinforcedSuitBody.GetComponent<Renderer>().material.mainTexture = suitEquipped ? _newRSuitBodyTexture : _vanillaRSuitBodyTexture;
            _reinforcedSuitBody.GetComponent<Renderer>().materials[1].mainTexture = suitEquipped ? _newRSuitArmsTexture : _vanillaRSuitArmsTexture;

        }

        public void UpdateIcon()
        {
            if(hudItemIcon.equippedTechTypes.Count > 1)
            {
                hudItemIcon.itemIcon.SetBackgroundSprite(_armorSuitSprite);
                hudItemIcon.itemIcon.SetForegroundSprite(_armorSuitSprite);
            }
            else
            {
                hudItemIcon.itemIcon.SetBackgroundSprite(_armorSuitSpriteGloveless);
                hudItemIcon.itemIcon.SetForegroundSprite(_armorSuitSpriteGloveless);
            }
        }

        
        public void UpdateDamageModifier()
        {
            var DR = 1f - (0.45f * hudItemIcon.equippedTechTypes.Count);//get 45% DR per item equipped
            SuitDefense.EditDamageModifiers(CurrentType, DR);
        }

        private void SetColors(Color color)
        {
            SetIconColor(color);
            UpdatePlayerSuit();
        }

        private void SetIconColor(Color color)
        {
            hudItemIcon.itemIcon.SetColors(color, color, color);
        }

        public void OnTakeDamage(DamageInfo damageInfo)
        {
            if (!QMod.config.Automatic || hudItemIcon.equippedTechTypes.Count == 0/* || UnaffectedTypes.Contains(damageInfo.type)*/) 
                return;

            var defenseInfo = defenseInfos.FirstOrDefault((info) => info.DamageTypes.Contains(damageInfo.type));//find the first damage info that covers this damage type
            if (defenseInfo != null)
            {
                CurrentType = defenseInfo;
            }
        }

        public enum DefenseType
        {
            Physical,
            Electrical,
            Thermal,
            Cold,
            Radioactive,
            Poisonous,
            Acidic,
            Explosive
        }
#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode() - too lazy 
#pragma warning disable CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()
        public struct DefenseInfo
        {
            [JsonIgnore]
            public Color SuitColor => new Color(red, green, blue);

            public float red;

            public float green;

            public float blue;

            public DefenseType Type;

            public List<DamageType> DamageTypes;
            //one defense type can cover multiple damage types
            //ie; Thermal covers both Heat and Fire

            public DefenseInfo(DefenseType type, Color color, List<DamageType> types)
            {
                //struct containing the necessary information for a specific defensive state
                Type = type;
                red = color.r;
                green = color.g;
                blue = color.b;
                DamageTypes = types;
            }



            public override bool Equals(object obj)
            {
                return obj is DefenseInfo && ((DefenseInfo)obj).Type == Type;//only need to compare the damage type here, nothing else will ever be needed
            }
            public static bool operator ==(DefenseInfo info, DefenseInfo info2) => info.Type == info2.Type;
            public static bool operator !=(DefenseInfo info, DefenseInfo info2) => info.Type != info2.Type;
        }
#pragma warning restore CS0661
#pragma warning restore CS0659
    }
}
