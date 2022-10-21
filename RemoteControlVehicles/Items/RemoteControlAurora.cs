using RemoteControlVehicles.Monobehaviours;
using SMLHelper.V2.Assets;
using SMLHelper.V2.Crafting;
using System.Collections.Generic;
using UnityEngine;
using UWE;

namespace RemoteControlVehicles.Items
{
    internal class RemoteControlAurora : Equipable
    {
        public RemoteControlAurora() : base("RemoteControlAurora", "Remote Control Aurora", "A Remote Controllable aurora miniature! It's so cute!")
        {

        }
        protected override Atlas.Sprite GetItemSprite() => SpriteManager.Get(TechType.StarshipSouvenir);
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
                    new Ingredient(TechType.StarshipSouvenir, 1),
                    new Ingredient(TechType.AdvancedWiringKit, 1)
                }
            };
        }

        public override GameObject GetGameObject()
        {
            var obj = CraftData.InstantiateFromPrefab(TechType.StarshipSouvenir);
            obj.AddComponent<RemoteControlAuroraTool>();
            obj.AddComponent<RemoteControlAuroraMono>();
            return obj;
        }
    }
}
