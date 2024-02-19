#if !SN1
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Crafting;
using Nautilus.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Sprite = Atlas.Sprite;
namespace EquippableItemIcons.API.SecretSMLNautilusAPIDontTouch
{
    [Obsolete("Don't touch this shit. I'm making this because I'm lazy, don't touch it. Just convert to custom prefabs and gadgets like everybody else, its the proper way to do things. Don't be a me.", false)]
    public abstract class Equipable
    {
        public TechType TechType { get; private set; }
        protected Action OnFinishedPatching;
        public virtual EquipmentType EquipmentType => EquipmentType.None;
        public abstract TechType RequiredForUnlock { get; }
        public virtual Vector2int SizeInInventory => new(1, 1);
        public virtual QuickSlotType QuickSlotType => QuickSlotType.Selectable;
        private string classid;
        private string friendlyName;
        private string description;
        public Equipable(string classid, string friendlyName, string description)
        {
            this.description = description;
            this.classid = classid;
            this.friendlyName = friendlyName;
        }

        protected abstract Sprite GetItemSprite();
        public virtual TechGroup GroupForPDA => TechGroup.Uncategorized;

        public virtual TechCategory CategoryForPDA => TechCategory.BasePiece;//Close enough to unused here

        public virtual CraftTree.Type FabricatorType => CraftTree.Type.None;

        public virtual string[] StepsToFabricatorTab => null;

        public virtual float CraftingTime => 3f;
        protected abstract RecipeData GetBlueprintRecipe();
        public abstract IEnumerator GetGameObjectAsync(IOut<GameObject> gameObject);
        public void Patch()
        {
            var prefab = new CustomPrefab(classid, friendlyName, description);

            prefab.SetGameObject(GetGameObjectAsync);
            if(FabricatorType != CraftTree.Type.None)
            {
                prefab.SetRecipe(GetBlueprintRecipe()).WithStepsToFabricatorTab(StepsToFabricatorTab).WithFabricatorType(FabricatorType).WithCraftingTime(CraftingTime);
            }
            if(EquipmentType != EquipmentType.None)
            {
                prefab.SetEquipment(EquipmentType).QuickSlotType = QuickSlotType;
            }
            if (GroupForPDA != TechGroup.Uncategorized)
            {
                prefab.SetPdaGroupCategory(GroupForPDA, CategoryForPDA);
            }

            prefab.Register();

            OnFinishedPatching?.Invoke();
        }
    }
}
#endif