using ArmorSuit.Mono;
using EquippableItemIcons.API;
using HarmonyLib;
#if SN1
using Oculus.Newtonsoft.Json;
#else
using Newtonsoft.Json;
#endif
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
#if SN
using Sprite = Atlas.Sprite;
#if SN1
using RecipeData = SMLHelper.V2.Crafting.TechData;
using SMLHelper.V2.Utility;
#else
using Nautilus.Utility;
#endif
#endif

namespace ArmorSuit
{
    public class ArmorSuitMono : MonoBehaviour, IOnTakeDamage
    {
#region Item Icon Stuff
        public static string AssetsFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");

        private static Sprite _armorSuitSprite = ImageUtils.LoadSpriteFromFile(Path.Combine(AssetsFolder, "ArmorSuitIcon.png"));
        private static Sprite _armorSuitSpriteGloveless = ImageUtils.LoadSpriteFromFile(Path.Combine(AssetsFolder, "armorsuitIcon-gloveless.png"));

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
            hudItemIcon.OnUnEquip += SetSuitColors;

            Registries.RegisterHudItemIcon(hudItemIcon);
        }
        private void SetUpSuitColors()
        {
            SuitColors = gameObject.EnsureComponent<SuitColors>();
            SuitColors.renderers = Player.main.transform.Find("body/player_view/male_geo/reinforcedSuit").GetComponentsInChildren<Renderer>(true);
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
            SetSuitColors();
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
            SetSuitColors();
        }

        private void SetSuitColors()
        {
            Color color = hudItemIcon.equippedTechTypes.Count == 0 ? Color.white : CurrentType.SuitColor;

            foreach (Renderer rend in SuitColors.renderers)
            {
                foreach (var item in rend.materials)
                {
                    item.color = color;
                    item.SetColor("_SpecColor", color);
                }
            }
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
