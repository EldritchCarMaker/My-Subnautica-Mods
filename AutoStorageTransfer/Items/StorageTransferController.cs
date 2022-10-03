using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AutoStorageTransfer.Monobehaviours;
using SMLHelper.V2.Assets;
using SMLHelper.V2.Crafting;
using SMLHelper.V2.Utility;
using UnityEngine;

namespace AutoStorageTransfer.Items
{
    internal class StorageTransferController : Equipable
    {
        public StorageTransferController() : base("StorageTransferController", "Storage Transfer Controller", "Allows control over storages, and setting them to transfer to or from other storages")
        {

        }

        public override EquipmentType EquipmentType => EquipmentType.Hand;

        protected override TechData GetBlueprintRecipe()
        {
            return new TechData()
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient(TechType.Magnetite, 1)
                }
            };
        }
        public override GameObject GetGameObject()
        {
            var obj = CraftData.InstantiateFromPrefab(TechType.Welder);
            GameObject.Destroy(obj.GetComponent<Welder>());
            GameObject.Destroy(obj.GetComponent<EnergyMixin>());
            obj.AddComponent<StorageTransferControllerMono>();
            return obj;
        }
        public override TechGroup GroupForPDA => TechGroup.Personal; 
        public override TechCategory CategoryForPDA => TechCategory.Tools;
        public override CraftTree.Type FabricatorType => CraftTree.Type.Fabricator;
        public override string[] StepsToFabricatorTab => new[] { "Personal", "Tools" };
        public override string AssetsFolder => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");
        protected override Atlas.Sprite GetItemSprite()
        {
            return ImageUtils.LoadSpriteFromFile(Path.Combine(AssetsFolder, "StorageTransferController.png"));
        }
    }
}
