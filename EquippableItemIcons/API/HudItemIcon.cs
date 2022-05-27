using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Logger = QModManager.Utility.Logger;

namespace EquippableItemIcons.API
{
    public class HudItemIcon
    {
        public string name;//self explanatory
        public Atlas.Sprite sprite;//foreground sprite of item icon
        public TechType techType;//used to determine if item is equipped or not

        public GameObject container;//try to avoid touching these if AutomaticSetup is true, otherwise go ham
        public GameObject itemIconObject;
        public uGUI_ItemIcon itemIcon;

        //whether we handle the item's charge, activation, and icon animation, or your mod does
        public bool AutomaticSetup = true;

        public bool equipped = false;//if item is equipped by player
        public List<TechType> equippedTechTypes = new List<TechType>();
        public EquipmentType equipmentType = EquipmentType.Chip;//the item slot its equipped into
        public bool iconActive = false;//if the icon is currently showing
        public bool InvertIcon = true;//if the container should be rotated upside down, used so that the animation fades from top to bottom rather than bottom to top. If true, the sprite should be rotated 180 degrees to compensate
        public bool fadeBackground = true;//whether the background sprite also fades, or if its only the foreground sprite

        public delegate void ToggleEvent();//any event with returns void
        public ToggleEvent OnEquip;//in case you need to do something special when item is equipped
        public ToggleEvent OnUnEquip;// case you need to do something special when item is unequipped

        public delegate bool AllowedEvent();//any event that returns a bool
        public AllowedEvent IsIconActive;//used for if there's a specific condition for when the icon should/shouldn't be active, has a default 

        public Atlas.Sprite backgroundSprite;

        public List<TechType> SecondaryTechTypes = new List<TechType>();//for if multiple item techtypes should use the same icon


        public HudItemIcon(string name, Atlas.Sprite sprite, TechType itemTechType)
        {
            this.name = name;
            this.sprite = sprite;
            techType = itemTechType;
        }
        internal void makeIcon()
        {
            uGUI_QuickSlots quickSlots = uGUI.main.quickSlots;

            container = new GameObject(name + "Container");
            container.transform.SetParent(quickSlots.transform, false);
            container.layer = quickSlots.gameObject.layer;

            itemIconObject = new GameObject(name);
            itemIconObject.transform.SetParent(container.transform, false);
            itemIconObject.layer = quickSlots.gameObject.layer;

            itemIcon = itemIconObject.AddComponent<uGUI_ItemIcon>();
            itemIcon.Init(null, container.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
            itemIcon.SetForegroundSprite(sprite);
            itemIcon.SetForegroundBlending(Blending.AlphaBlend);

            itemIcon.CreateBackground();
            itemIcon.SetBackgroundSprite(backgroundSprite);

            if (InvertIcon)
                container.transform.eulerAngles = new Vector3(0, 180, 180);

            if (!AutomaticSetup)
            {
                iconActive = equipped;
                container.SetActive(iconActive);
                Registries.UpdatePositions();
                Logger.Log(Logger.Level.Info, $"Finished setup of {name}");
                return;
            }


            itemIcon.SetProgress(1, FillMethod.Vertical);

            equipped = UtilityStuffs.Utility.EquipmentHasItem(techType, equipmentType);

            iconActive = equipped;
            container.SetActive(iconActive);
            Registries.UpdatePositions();
            Logger.Log(Logger.Level.Info, $"Finished setup of {name}");
        }

        internal void UpdateEquipped()
        {
            if (AutomaticSetup)
            {
                equippedTechTypes.Clear();

                var temp = UtilityStuffs.Utility.EquipmentHasItem(techType, equipmentType);

                if (temp) equippedTechTypes.Add(techType);

                if (SecondaryTechTypes != null && SecondaryTechTypes.Count > 0)
                {
                    foreach (TechType type in SecondaryTechTypes)
                    {
                        if (UtilityStuffs.Utility.EquipmentHasItem(type, equipmentType))
                        {
                            temp = true;
                            equippedTechTypes.Add(type);
                        }
                    }
                }

                if (temp != equipped)
                {
                    if (temp && OnEquip != null) OnEquip.Invoke();
                    else if (!temp && OnUnEquip != null) OnUnEquip.Invoke();
                }
                equipped = temp;

                if (InvertIcon)
                {
                    if (container != null && container.transform != null)
                    {
                        container.transform.eulerAngles = new Vector3(0, 180, 180);//for some reason the angle would be off unless I set it here
                    }
                    else
                    {
                        //I still want to know about this, but it also is run every time when the game quits so I only want to know outside of the game being quit
                        //Logger.Log(Logger.Level.Warn, $"icon Container null: {container == null}, {(container == null ? "" : $"Transform null: {container.transform != null}, " )} If you get this message, ping Nagorrogan in the subnautica modding discord and send the log file to me");
                    }
                }
            }
        }
        internal /*virtual*/ void Update()
        {
            iconActive = IsIconActive != null ? IsIconActive.Invoke() : equipped;
        }
    }
}
