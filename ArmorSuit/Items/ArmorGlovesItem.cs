using SMLHelper.V2.Assets;
using SMLHelper.V2.Crafting;
using SMLHelper.V2.Utility;
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
using RecipeData = SMLHelper.V2.Crafting.TechData;
#endif

namespace ArmorSuit
{
    internal class ArmorGlovesItem : Equipable
    {
        public static TechType techType { get; private set; }

        public override EquipmentType EquipmentType => EquipmentType.Gloves;
        public override TechType RequiredForUnlock => TechType.Unobtanium;
        public override Vector2int SizeInInventory => new Vector2int(2, 2);

        public ArmorGlovesItem() : base("ArmorGlovesItem", "Armor Gloves", "A high tech adaptive pair of gloves which gives high damage reduction to a specific damage type")
        {
            OnFinishedPatching += () => techType = TechType;
        }

        protected override Sprite GetItemSprite()
        {
            return ImageUtils.LoadSpriteFromFile(Path.Combine(ArmorSuitMono.AssetsFolder, "ArmorGloves.png"));
        }

        protected override RecipeData GetBlueprintRecipe()
        {
            return new RecipeData();
        }
#if SN1
public override GameObject GetGameObject()
        {
            return CraftData.InstantiateFromPrefab(TechType.ReinforcedGloves);
        }
#endif
        public override IEnumerator GetGameObjectAsync(IOut<GameObject> gameObject)
        {
            var task = CraftData.GetPrefabForTechTypeAsync(TechType.ReinforcedGloves);
            yield return task;
            gameObject.Set(task.GetResult());
        }
    }
}
