using RemoteControlVehicles.Monobehaviours;
using SMLHelper.V2.Assets;
using SMLHelper.V2.Crafting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UWE;

namespace RemoteControlVehicles.Items
{
    internal class RemoteControlCar : Equipable
    {
        public RemoteControlCar() : base("RemoteControlCar", "Remote Control Car", "A Remote Controllable toy car! Just like the mini aurora, it's super cute!")
        {

        }
        protected override Atlas.Sprite GetItemSprite() => SpriteManager.Get(TechType.ToyCar);
        public override EquipmentType EquipmentType => EquipmentType.Hand;
        public override WorldEntityInfo EntityInfo => new WorldEntityInfo()
        {
            cellLevel = LargeWorldEntity.CellLevel.Global,
            classId = ClassID,
            localScale = new Vector3(1, 1, 1),
            prefabZUp = true,
            slotType = EntitySlot.Type.Small,
            techType = TechType
        };
        public override TechType RequiredForUnlock => TechType.BaseMapRoom;
        public override TechGroup GroupForPDA => TechGroup.Personal;
        public override TechCategory CategoryForPDA => TechCategory.Tools;
        public override CraftTree.Type FabricatorType => CraftTree.Type.Fabricator;
        public override string[] StepsToFabricatorTab => new[] { "Personal", "Tools" };
        protected override TechData GetBlueprintRecipe()
        {
            return new TechData()
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient(TechType.ToyCar, 1),
                    new Ingredient(TechType.AdvancedWiringKit, 1)
                }
            };
        }

        public override GameObject GetGameObject()
        {
            if (!PrefabDatabase.TryGetPrefab("dfabc84e-c4c5-45d9-8b01-ca0eaeeb8e65", out var prefab))
                return null;

            var obj = GameObject.Instantiate(prefab);

            obj.AddComponent<RemoteControlCarTool>();
            obj.AddComponent<RemoteControlCarMono>();
            return obj;
        }
    }
}
