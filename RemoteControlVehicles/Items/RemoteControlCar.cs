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
using UWE;
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
    public class RemoteControlCar : Equipable
#else
    public class RemoteControlCar
#endif
    {
        public static void Patch()
        {
#if !SN1
            var prefab = new CustomPrefab("RemoteControlCar", "Remote Control Car", "A Remote Controllable toy car! Just like the mini aurora, it's super cute!", SpriteManager.Get(TechType.ToyCar));
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

#else

        public static IEnumerator GetGameObject(IOut<GameObject> @out)
        {
            var task = PrefabDatabase.GetPrefabAsync("dfabc84e-c4c5-45d9-8b01-ca0eaeeb8e65");
            yield return task;
            task.TryGetPrefab(out var prefab);
            var taskCam = CraftData.GetPrefabForTechTypeAsync(TechType.MapRoomCamera);
            yield return taskCam;
            var camPrefab = taskCam.GetResult().GetComponent<MapRoomCamera>();

            var obj = GameObject.Instantiate(prefab);
            obj.SetActive(false);

            obj.AddComponent<RemoteControlCarTool>();
            var mono = obj.AddComponent<RemoteControlCarMono>();



            mono.lightsParent = GameObject.Instantiate(camPrefab.lightsParent);
            mono.lightsParent.SetActive(false);
            mono.lightsParent.transform.parent = obj.transform;
            mono.lightsParent.transform.localRotation = Quaternion.identity;
            mono.lightsParent.transform.localPosition = Vector3.zero;

            var energyMixin = obj.EnsureComponent<EnergyMixin>();
            energyMixin.compatibleBatteries = camPrefab.energyMixin.compatibleBatteries;
            energyMixin.storageRoot = (new GameObject("BatteryRoot").AddComponent<ChildObjectIdentifier>());
            energyMixin.defaultBattery = TechType.Battery;
            energyMixin.batteryModels = new EnergyMixin.BatteryModels[0];
            energyMixin.controlledObjects = new GameObject[0];


            var toggleLights = obj.EnsureComponent<ToggleLights>();
            toggleLights.energyMixin = energyMixin;


            obj.EnsureComponent<LargeWorldEntity>().cellLevel = LargeWorldEntity.CellLevel.Global;


            var liveMixin = obj.EnsureComponent<LiveMixin>();
            liveMixin.data = ScriptableObject.CreateInstance<LiveMixinData>();
            liveMixin.data.maxHealth = 100;
            liveMixin.data.destroyOnDeath = false;
            liveMixin.data.canResurrect = true;
            liveMixin.data.weldable = true;
            liveMixin.ResetHealth();
            var pickupable = obj.EnsureComponent<Pickupable>();


            var rigidBody = obj.EnsureComponent<Rigidbody>();
            rigidBody.angularDrag = 1;


            var worldForces = obj.EnsureComponent<WorldForces>();

            @out.Set(obj);
        }
        protected static RecipeData GetRecipe()
        {
            return new RecipeData()
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient(TechType.ToyCar, 1),
                    new Ingredient(TechType.AdvancedWiringKit, 1)
                }
            };
        }

#endif
    }
}
