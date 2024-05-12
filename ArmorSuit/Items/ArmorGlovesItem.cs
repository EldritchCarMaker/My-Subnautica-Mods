using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
#if SN
using Sprite = Atlas.Sprite;
using Nautilus.Assets.PrefabTemplates;

using Nautilus.Assets;
using Nautilus.Assets.Gadgets;


#if SN1
using RecipeData = SMLHelper.V2.Crafting.TechData;
using SMLHelper.V2.Assets;
using SMLHelper.V2.Crafting;
using SMLHelper.V2.Utility;
#else
using Nautilus.Crafting;
using Nautilus.Utility;
#endif
#endif

namespace ArmorSuit
{
    internal class ArmorGlovesItem
    {
        public static TechType techType { get; private set; }
        public static void Patch()
        {
            var customPrefab = new CustomPrefab("ArmorGlovesItem", "Armor Gloves", "A high tech adaptive pair of gloves which gives high damage reduction to a specific damage type", GetItemSprite());
            customPrefab.SetGameObject(new CloneTemplate(customPrefab.Info, TechType.ReinforcedGloves));
            customPrefab.Info.WithSizeInInventory(new(2, 2));

            customPrefab.SetEquipment(EquipmentType.Gloves);


            customPrefab.Register();
            techType = customPrefab.Info.TechType;
        }

        public static Sprite GetItemSprite()
        {
            return ImageUtils.LoadSpriteFromFile(Path.Combine(ArmorSuitMono.AssetsFolder, "ArmorGloves.png"));
        }
    }
}
