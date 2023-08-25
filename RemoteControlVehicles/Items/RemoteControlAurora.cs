using RemoteControlVehicles.Monobehaviours;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static CraftData;
#if SN1
using SMLHelper.V2.Assets;
using SMLHelper.V2.Crafting;
using SMLHelper.V2.Utility;
using RecipeData = SMLHelper.V2.Crafting.TechData;
#else
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Crafting;
using Nautilus.Utility;
#endif
#if SN
using Sprite = Atlas.Sprite;
#endif

namespace RemoteControlVehicles.Items
{
#if SN1
    public class RemoteControlAurora : Equipable
#else
    public class RemoteControlAurora
#endif

    {
        public static void Patch()
        {
#if !SN1
            var prefab = new CustomPrefab("RemoteControlAurora", "Remote Control Aurora", "A Remote Controllable aurora miniature! It's so cute!", SpriteManager.Get(TechType.StarshipSouvenir));
            prefab.SetGameObject(GetGameObject);
            prefab.SetEquipment(EquipmentType.Hand).WithQuickSlotType(QuickSlotType.Selectable);
            prefab.SetPdaGroupCategory(TechGroup.Personal, TechCategory.Tools);
            prefab.SetUnlock(TechType.BaseMapRoom);
            prefab.SetRecipe(GetRecipe()).WithCraftingTime(5).WithStepsToFabricatorTab(new[] { "Personal", "Tools" }).WithFabricatorType(CraftTree.Type.Fabricator);
            //GadgetExtensions.do shti

            prefab.Register();

#else
            ((Equipable)new DroneControlRemote()).Patch();
#endif
        }
#if SN1
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
#else
        protected static RecipeData GetRecipe()
        {
            return new RecipeData()
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient(TechType.StarshipSouvenir, 1),
                    new Ingredient(TechType.AdvancedWiringKit, 1)
                }
            };
        }

        public static IEnumerator GetGameObject(IOut<GameObject> @out)
        {
            var task = CraftData.GetPrefabForTechTypeAsync(TechType.StarshipSouvenir);
            yield return task;

            var obj = GameObject.Instantiate(task.GetResult());
            obj.AddComponent<RemoteControlAuroraTool>();
            obj.AddComponent<RemoteControlAuroraMono>();
            @out.Set(obj);
        }
#endif
    }
}