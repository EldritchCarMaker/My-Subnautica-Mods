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
    public class DroneControlRemote : Equipable
#else
    public class DroneControlRemote
#endif

    {
        public static string AssetsFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");
        public static void Patch()
        {
#if !SN1
            var prefab = new CustomPrefab("DroneControlRemote", "Remote Control Remote", "Allows on-the-go remote control over vehicles and scanner room camera drones", ImageUtils.LoadSpriteFromFile(Path.Combine(AssetsFolder, "SeamothRemoteControl.png")));
            prefab.SetGameObject(GetGameObject);
            prefab.SetEquipment(EquipmentType.Hand).WithQuickSlotType(QuickSlotType.Selectable);
            prefab.SetPdaGroupCategory(TechGroup.Personal, TechCategory.Tools);
            prefab.SetUnlock(TechType.BaseMapRoom); prefab.SetRecipe(GetRecipe()).WithCraftingTime(5).WithStepsToFabricatorTab(new[] { "Personal", "Tools" }).WithFabricatorType(CraftTree.Type.Fabricator);

            prefab.Register();

#else
            ((Equipable)new DroneControlRemote()).Patch();
#endif
        }
        public static TechType thisTechType;

#if SN1

        public DroneControlRemote() : base("DroneControlRemote", "Remote Control Remote", "Allows on-the-go remote control over vehicles and scanner room camera drones")
        {
            OnFinishedPatching += () =>
            {
                thisTechType = TechType;
            };
        }

        public override EquipmentType EquipmentType => EquipmentType.Hand;
        public override TechType RequiredForUnlock => TechType.BaseMapRoom;
        public override TechGroup GroupForPDA => TechGroup.Personal;
        public override TechCategory CategoryForPDA => TechCategory.Tools;
        public override CraftTree.Type FabricatorType => CraftTree.Type.Fabricator;
        public override string[] StepsToFabricatorTab => new[] { "Personal", "Tools" };
        public override float CraftingTime => 5f;
        public override QuickSlotType QuickSlotType => QuickSlotType.Selectable;
        protected override Sprite GetItemSprite()
        {
            return ImageUtils.LoadSpriteFromFile(Path.Combine(AssetsFolder, "HudChipRemoteControl.png"));
        }

        protected override RecipeData GetBlueprintRecipe()
        {
            return GetRecipe();
        }

        public override GameObject GetGameObject()
        {
            var prefab = CraftData.GetPrefabForTechType(TechType.AirBladder);
            var obj = GameObject.Instantiate(prefab);

            GameObject.Destroy(obj.GetComponent<AirBladder>());
            obj.AddComponent<DroneControl>();

            return obj;
        }
#endif
        public static IEnumerator GetGameObject(IOut<GameObject> @out)
        {
            var task = CraftData.GetPrefabForTechTypeAsync(TechType.AirBladder);
            yield return task;
            var prefab = task.GetResult();

            var obj = GameObject.Instantiate(prefab);

            GameObject.Destroy(obj.GetComponent<AirBladder>());
            obj.AddComponent<DroneControl>();

            @out.Set(obj);
        }
        public static RecipeData GetRecipe()
        {

            return new RecipeData()
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>(new Ingredient[]
                    {
                        new Ingredient(TechType.Magnetite, 2),
                        new Ingredient(TechType.ComputerChip, 2),
                        new Ingredient(TechType.WiringKit, 1)

                    }
                )
            };
        }
    }
}
