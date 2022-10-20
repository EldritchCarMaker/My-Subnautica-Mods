using SMLHelper.V2.Assets;
using SMLHelper.V2.Crafting;
using SMLHelper.V2.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CameraDroneUpgrades.API
{
    public class CameraDroneUpgradeModule : Craftable
    {
        public string assetsFolder;
        public override string AssetsFolder => assetsFolder;

        public CameraDroneUpgradeModule(string classId, string friendlyName, string description) : base(classId, friendlyName, description)
        {
        }
        public CameraDroneUpgradeModule(string classId, string friendlyName, string description, Atlas.Sprite sprite) : base(classId, friendlyName, description)
        {
            this.sprite = sprite;
        }
        public CameraDroneUpgradeModule(string classId, string friendlyName, string description, TechType RequiredForUnlock) : base(classId, friendlyName, description)
        {
            this.requiredForUlock = RequiredForUnlock;
        }
        public CameraDroneUpgradeModule(string classId, string friendlyName, string description, TechData techData) : base(classId, friendlyName, description)
        {
            this.techData = techData;
        }
        public CameraDroneUpgradeModule(string classId, string friendlyName, string description, Atlas.Sprite sprite, TechType RequiredForUnlock) : base(classId, friendlyName, description)
        {
            this.sprite = sprite;
            this.requiredForUlock = RequiredForUnlock;
        }
        public CameraDroneUpgradeModule(string classId, string friendlyName, string description, Atlas.Sprite sprite, TechData techData) : base(classId, friendlyName, description)
        {
            this.sprite = sprite;
            this.techData = techData;
        }
        public CameraDroneUpgradeModule(string classId, string friendlyName, string description, TechType RequiredForUnlock, TechData techData) : base(classId, friendlyName, description)
        {
            this.requiredForUlock = RequiredForUnlock;
            this.techData = techData;
        }
        public CameraDroneUpgradeModule(string classId, string friendlyName, string description, Atlas.Sprite sprite, TechType RequiredForUnlock, TechData techData) : base(classId, friendlyName, description)
        {
            this.sprite = sprite;
            this.requiredForUlock = RequiredForUnlock;
            this.techData = techData;
        }

        public Atlas.Sprite sprite;

        public TechType requiredForUlock = TechType.BaseMapRoom;
        public override TechType RequiredForUnlock => requiredForUlock;
        public override TechGroup GroupForPDA => TechGroup.Personal;
        public override TechCategory CategoryForPDA => TechCategory.Equipment;
        public override CraftTree.Type FabricatorType => CraftTree.Type.MapRoom;
        public override string[] StepsToFabricatorTab => Registrations.upgradeModulePaths;
        public override float CraftingTime => 3f;
        protected override Atlas.Sprite GetItemSprite() => sprite;
        public TechData techData;
        protected override TechData GetBlueprintRecipe()
        {
            return techData;
        }

        public override GameObject GetGameObject()
        {
            var prefab = CraftData.GetPrefabForTechType(TechType.MapRoomHUDChip);
            var obj = GameObject.Instantiate(prefab);
            return obj;
        }
    }
}
