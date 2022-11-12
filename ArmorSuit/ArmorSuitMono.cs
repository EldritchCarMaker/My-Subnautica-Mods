using EquippableItemIcons.API;
using HarmonyLib;
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

        private static readonly List<DamageType> UnaffectedTypes = new List<DamageType>()
        {
            DamageType.Undefined,
            DamageType.Pressure,
            DamageType.LaserCutter,
            DamageType.Starve,
            DamageType.Smoke,
        };
        private static readonly Dictionary<DamageType, DamageModifier> DamageModifiers = new Dictionary<DamageType, DamageModifier>();

        private static List<DefenseInfo> defenseInfos = new List<DefenseInfo>();

        private DefenseInfo _currentType;

        public DefenseInfo CurrentType
        {
            get 
            {
                if (_currentType == null || _currentType.DamageTypes == null) CurrentType = defenseInfos[0];
                return _currentType; 
            }

            set
            {
                if(_currentType != value) ErrorMessage.AddMessage("Defense type now " + value.Type);
                _currentType = value;
                UpdateDamageModifier();
                SetIconColor(CurrentType.SuitColor);
            }
        }

        public void Awake()
        {
            PopulateDefenseTypes();

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

        #region Defense Type Field Population
        private void PopulateDefenseTypes()
        {
            defenseInfos.Add
            (
                new DefenseInfo(
                    DefenseType.Physical,
                    new Color(0.6f, 0.6f, 0.6f),
                    new List<DamageType>()
                    {
                        DamageType.Normal,
                        DamageType.Collide,
                        DamageType.Puncture,
                        DamageType.Drill
                    }
               )
            );

            defenseInfos.Add
            (
                new DefenseInfo(
                    DefenseType.Electrical,
                    new Color(0, 0.235f, 1f),
                    new List<DamageType>()
                    {
                        DamageType.Electrical
                    }
               )
            );
            
            defenseInfos.Add
            (
                new DefenseInfo(
                    DefenseType.Thermal,
                    new Color(1, 0.314f, 0),
                    new List<DamageType>()
                    {
                        DamageType.Heat,
                        DamageType.Fire
                    }
               )
            );

            defenseInfos.Add
            (
                new DefenseInfo(
                    DefenseType.Acidic,
                    new Color(0, 0.75f, 0),
                    new List<DamageType>()
                    {
                        DamageType.Acid
                    }
               )
            );

            defenseInfos.Add
            (
                new DefenseInfo(
                    DefenseType.Poisonous,
                    new Color(0, 1, 0),
                    new List<DamageType>()
                    {
                        DamageType.Poison,
                    }
               )
            );

            defenseInfos.Add
            (
                new DefenseInfo(
                    DefenseType.Cold,
                    new Color(0, 0.725f, 1),
                    new List<DamageType>()
                    {
                        DamageType.Cold,
                    }
               )
            );

            defenseInfos.Add
            (
                new DefenseInfo(
                    DefenseType.Radioactive,
                    new Color(1, 1, 0),
                    new List<DamageType>()
                    {
                        DamageType.Radiation,
                    }
               )
            );

            defenseInfos.Add
            (
                new DefenseInfo(
                    DefenseType.Explosive,
                    new Color(1, 0, 0),
                    new List<DamageType>()
                    {
                        DamageType.Explosive,
                    }
               )
            );

            SetUpDamageModifiers();
        }
        private void SetUpDamageModifiers()
        {
            foreach(DamageType type in Enum.GetValues(typeof(DamageType)))
            {
                if(!UnaffectedTypes.Contains(type))//too lazy to copy paste all the types I do want. easier to filter the ones I don't 
                {
                    AddDamageModifier(type);
                }
            }
        }
        private void AddDamageModifier(DamageType type)
        {
            var modifier = Player.main.gameObject.AddComponent<DamageModifier>();
            modifier.damageType = type;

            DamageModifiers.Add(type, modifier);
        }
#endregion

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
            EditDamageModifiers(CurrentType, DR);
        }

        private void EditDamageModifiers(DefenseInfo defenseInfo, float multiplier)
        {
            foreach(var pair in DamageModifiers)
            {
                if(defenseInfo.DamageTypes.Contains(pair.Key))
                {
                    pair.Value.multiplier = multiplier;
                }
                else
                {
                    pair.Value.multiplier = 1;
                }
            }
        }

        private void SetIconColor(Color color)
        {
            hudItemIcon.itemIcon.SetColors(color, color, color);
        }

        public void OnTakeDamage(DamageInfo damageInfo)
        {
            if (!QMod.config.Automatic || hudItemIcon.equippedTechTypes.Count == 0 || UnaffectedTypes.Contains(damageInfo.type)) 
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
            public DefenseType Type;

            public Color SuitColor;

            public List<DamageType> DamageTypes;
            //one defense type can cover multiple damage types
            //ie; Thermal covers both Heat and Fire

            public DefenseInfo(DefenseType type, Color color, List<DamageType> types)
            {
                //struct containing the necessary information for a specific defensive state
                Type = type;
                SuitColor = color;
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
