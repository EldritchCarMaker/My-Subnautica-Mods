using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AutoStorageTransfer.Monobehaviours;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Crafting;
using Nautilus.Utility;
using UnityEngine;
using static CraftData;
#if SN1
using RecipeData = SMLHelper.V2.Crafting.TechData;
#endif
#if SN
using Sprite = Atlas.Sprite;
#endif

namespace AutoStorageTransfer.Items
{
    internal class StorageTransferController
    {

        protected static RecipeData GetBlueprintRecipe()
        {
            return new RecipeData()
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient(TechType.Magnetite, 1)
                }
            };
        }
#if SN1
        public override GameObject GetGameObject()
        {
            var prefab = CraftData.GetPrefabForTechType(TechType.Welder);
#else
        public static IEnumerator GetGameObjectAsync(IOut<GameObject> gameObject)
        {
            var task = CraftData.GetPrefabForTechTypeAsync(TechType.Welder);
            yield return task;
            var prefab = task.GetResult();
#endif
            var obj = GameObject.Instantiate(prefab);

            GameObject.Destroy(obj.GetComponent<Welder>());
            GameObject.Destroy(obj.GetComponent<EnergyMixin>());
            obj.AddComponent<StorageTransferControllerMono>();
#if SN1
            return obj;
#else 
            gameObject.Set(obj);
#endif
        }
        public static string AssetsFolder => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");
        protected static Sprite GetItemSprite()
        {
            return ImageUtils.LoadSpriteFromFile(Path.Combine(AssetsFolder, "StorageTransferController.png"));
        }
        public static void Patch()
        {
            var prefab = new CustomPrefab("StorageTransferController", "Storage Transfer Controller", "Allows control over storages, and setting them to transfer to or from other storages", GetItemSprite());

            prefab.SetRecipe(GetBlueprintRecipe()).WithFabricatorType(CraftTree.Type.Fabricator).WithStepsToFabricatorTab(new[] { "Personal", "Tools" });
            prefab.SetUnlock(TechType.Magnetite).WithPdaGroupCategory(TechGroup.Personal, TechCategory.Tools);
            prefab.SetEquipment(EquipmentType.Hand).WithQuickSlotType(QuickSlotType.Selectable);

            prefab.SetGameObject(GetGameObjectAsync);

            prefab.Register();
        }
    }
}
