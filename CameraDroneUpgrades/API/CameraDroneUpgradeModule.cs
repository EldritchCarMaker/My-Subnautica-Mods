using SMLHelper.V2.Assets;
using SMLHelper.V2.Crafting;
using SMLHelper.V2.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
#if SN
using Sprite = Atlas.Sprite;
using RecipeData = SMLHelper.V2.Crafting.TechData;
#endif

namespace CameraDroneUpgrades.API
{
    public class CameraDroneUpgradeModule : Craftable
    {
        public string assetsFolder;
        public override string AssetsFolder => assetsFolder;

        public CameraDroneUpgradeModule(string classId, string friendlyName, string description) : base(classId, friendlyName, description)
        {
        }
        public CameraDroneUpgradeModule(string classId, string friendlyName, string description, Sprite sprite) : base(classId, friendlyName, description)
        {
            this.sprite = sprite;
        }
        public CameraDroneUpgradeModule(string classId, string friendlyName, string description, TechType RequiredForUnlock) : base(classId, friendlyName, description)
        {
            this.requiredForUlock = RequiredForUnlock;
        }
        public CameraDroneUpgradeModule(string classId, string friendlyName, string description, RecipeData techData) : base(classId, friendlyName, description)
        {
            this.techData = techData;
        }
        public CameraDroneUpgradeModule(string classId, string friendlyName, string description, Sprite sprite, TechType RequiredForUnlock) : base(classId, friendlyName, description)
        {
            this.sprite = sprite;
            this.requiredForUlock = RequiredForUnlock;
        }
        public CameraDroneUpgradeModule(string classId, string friendlyName, string description, Sprite sprite, RecipeData techData) : base(classId, friendlyName, description)
        {
            this.sprite = sprite;
            this.techData = techData;
        }
        public CameraDroneUpgradeModule(string classId, string friendlyName, string description, TechType RequiredForUnlock, RecipeData techData) : base(classId, friendlyName, description)
        {
            this.requiredForUlock = RequiredForUnlock;
            this.techData = techData;
        }
        public CameraDroneUpgradeModule(string classId, string friendlyName, string description, Sprite sprite, TechType RequiredForUnlock, RecipeData techData) : base(classId, friendlyName, description)
        {
            this.sprite = sprite;
            this.requiredForUlock = RequiredForUnlock;
            this.techData = techData;
        }

        public Sprite sprite;

        public TechType requiredForUlock = TechType.BaseMapRoom;
        public override TechType RequiredForUnlock => requiredForUlock;
        public override TechGroup GroupForPDA => TechGroup.Personal;
        public override TechCategory CategoryForPDA => TechCategory.Equipment;
        public override CraftTree.Type FabricatorType => CraftTree.Type.MapRoom;
        public override string[] StepsToFabricatorTab => Registrations.upgradeModulePaths;
        public override float CraftingTime => 3f;
        protected override Sprite GetItemSprite() => sprite;
        public RecipeData techData;
        protected override RecipeData GetBlueprintRecipe()
        {
            return techData;
        }
#if SN1
        public override GameObject GetGameObject()
        {
            var prefab = CraftData.GetPrefabForTechType(TechType.MapRoomHUDChip);
            var obj = GameObject.Instantiate(prefab);
            return obj;
        }
#endif
        public override IEnumerator GetGameObjectAsync(IOut<GameObject> gameObject)
        {
            var task = CraftData.GetPrefabForTechTypeAsync(TechType.MapRoomHUDChip);
            yield return task;
            gameObject.Set(GameObject.Instantiate(task.GetResult()));
        }
    }
}
