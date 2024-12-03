using System.Collections;
using UnityEngine;
using Nautilus.Assets;
using Nautilus.Crafting;
using Sprite = Atlas.Sprite;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Assets.Gadgets;
using System.Reflection;
using System.IO;
using Nautilus.Utility;

namespace CameraDroneUpgrades.API
{
    public class CameraDroneUpgradeModule
    {
        public CustomPrefab customPrefab;
        public PrefabInfo Info => customPrefab.Info;
        public TechType TechType => Info.TechType;

        public CameraDroneUpgradeModule(string classId, string friendlyName, string description, Sprite sprite = null, TechType RequiredForUnlock = TechType.Unobtanium, RecipeData techData = null)
        {
            customPrefab = new CustomPrefab(classId, friendlyName, description);

            //Give users a bit more time to set these
            this.sprite = sprite;
            this.requiredForUlock = RequiredForUnlock;
            this.techData = techData;
        }

        public Sprite sprite;
        public string assetsFolder;
        public string assetsName;

        public TechType requiredForUlock = TechType.BaseMapRoom;
        public TechGroup GroupForPDA = TechGroup.Personal;
        public TechCategory CategoryForPDA = TechCategory.Equipment;
        public CraftTree.Type FabricatorType = CraftTree.Type.MapRoom;
        public string[] StepsToFabricatorTab = Registrations.upgradeModulePaths;
        public float CraftingTime = 3f;
        public RecipeData techData;

        public void Patch(bool setAssetsFolder = true, bool setIcon = true)
        {
            if(setAssetsFolder && string.IsNullOrEmpty(assetsFolder))
            {
                assetsFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetCallingAssembly().Location), "Assets");
            }
            if (setIcon && sprite == null)
            {
                sprite = ImageUtils.LoadSpriteFromFile(Path.Combine(assetsFolder, assetsName + ".png"));
            }

            customPrefab.SetGameObject(new CloneTemplate(Info, TechType.MapRoomHUDChip));

            customPrefab.SetRecipe(techData).WithStepsToFabricatorTab(StepsToFabricatorTab).WithCraftingTime(CraftingTime).WithFabricatorType(FabricatorType);
            customPrefab.SetPdaGroupCategory(GroupForPDA, CategoryForPDA);
            customPrefab.SetUnlock(requiredForUlock);
            customPrefab.Info.WithIcon(sprite);

            customPrefab.Register();
        }
    }
}
