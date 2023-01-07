using Story;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UWE;
using static CyclopsVehicleUpgradeConsole.VehicleConsoleCreation;
using BepInEx.Bootstrap;
using System.Linq;
#if SN2
using BepInEx;
#endif

namespace CyclopsVehicleUpgradeConsole.Monobehaviours
{
    public class MakeVehicleButton : MonoBehaviour
    {
        //on the off chance easy craft gets changed, or I get one of these wrong during testing, easy to fix
        public const string easyCraftAssemblyName = "EasyCraft";
        public const string easyCraftMainClass = "EasyCraft.Main";
        public const string easyCraftRecipeFulfilledMethodName = "_IsCraftRecipeFulfilledAdvanced";
        public const string easyCraftConsumeMethodName = "ConsumeIngredients";


        public TechType vehicleType = TechType.Seamoth;
        public bool colorScreenActive = false;
        private uGUI_ItemIcon _ItemIcon;

        public uGUI_ItemIcon itemIcon { 
            get { 
                if (_ItemIcon == null) _ItemIcon = GetComponent<uGUI_ItemIcon>();
                return _ItemIcon;
            } 
            private set { _ItemIcon = value; } 
        }
        public void MakeVehicle()
        {
            SubRoot subRoot = this.gameObject.GetComponentInParent<SubRoot>();
            VehicleDockingBay vehicleDockingBay = subRoot.GetComponentInChildren<VehicleDockingBay>();
            if (vehicleDockingBay.dockedVehicle != null)
            {
                ErrorMessage.AddMessage("There is already a vehicle docked!");
                SetActive(gameObject);
                return;
            }


            if (!CrafterLogic.IsCraftRecipeUnlocked(vehicleType) && GameModeUtils.RequiresBlueprints())
            {
                ErrorMessage.AddMessage("You haven't unlocked the blueprint for this vehicle");
                FMODUWE.PlayOneShot(uGUI.main.craftingMenu.soundDeny, MainCamera.camera.transform.position, 1f);
                return;
            }
            if (!GameModeUtils.RequiresIngredients())
            {
                CoroutineHost.StartCoroutine(OnCraftingBegin(this.vehicleType, 5f));
                return;
            }

#if SN1
            if (QModManager.API.QModServices.Main.ModPresent("EasyCraft"))
            {
                if(EasyCraftMethods())
                    return;
                //if there's some problem when getting/using the easy craft methods, want to just continue with the manual way to do it.
            }
#else
            if (Chainloader.PluginInfos.ContainsKey("sn.easycraft.mod"))
            {
                if (EasyCraftMethods())
                    return;
                //if there's some problem when getting/using the easy craft methods, want to just continue with the manual way to do it.
            }
#endif


            if (!CrafterLogic.ConsumeResources(this.vehicleType))
            {
                FMODUWE.PlayOneShot(uGUI.main.craftingMenu.soundDeny, MainCamera.camera.transform.position, 1f);
                return;
            }
            CoroutineHost.StartCoroutine(OnCraftingBegin(this.vehicleType, 5f));
        }

        public bool EasyCraftMethods()
        {
            MethodInfo isCraftFulfilled = Helpers.FindMethod(easyCraftAssemblyName, easyCraftMainClass, easyCraftRecipeFulfilledMethodName);

            if(isCraftFulfilled == null)
            {
                ErrorMessage.AddMessage($"Easy craft installed but can't find method! Missing method: {easyCraftRecipeFulfilledMethodName}");
                return false;
            }

            Dictionary<TechType, int> consumable = new Dictionary<TechType, int>();
            Dictionary<TechType, int> crafted = new Dictionary<TechType, int>();

            if(!(bool)isCraftFulfilled.Invoke(null, new object[] { vehicleType, vehicleType, consumable, crafted, 0 }))
            {
                FMODUWE.PlayOneShot(uGUI.main.craftingMenu.soundDeny, MainCamera.camera.transform.position, 1f);
                ErrorMessage.AddMessage("You don't have the required materials!");
                return false;
            }

            MethodInfo consumeResources = Helpers.FindMethod(easyCraftAssemblyName, easyCraftMainClass, easyCraftConsumeMethodName);
            if(consumeResources == null)
            {
                ErrorMessage.AddMessage($"Easy craft installed but can't find method! Missing method: {easyCraftConsumeMethodName}");
                return false;
            }

            consumeResources.Invoke(null, new object[] { consumable });
            CoroutineHost.StartCoroutine(OnCraftingBegin(this.vehicleType, 5f));
            return true;
        }

        public IEnumerator OnCraftingBegin(TechType techType, float duration)
        {
            Vector3 zero = Vector3.zero;
            Quaternion identity = Quaternion.identity;

            //this.GetCraftTransform(techType, ref zero, ref identity);
            

            GameObject gameObject;

            var task = CraftData.GetPrefabForTechTypeAsync(techType);
            yield return task;
            var prefab = task.GetResult();

            gameObject = GameObject.Instantiate(prefab);
            Transform component = gameObject.GetComponent<Transform>();
            component.position = zero;
            component.rotation = identity;

            CrafterLogic.NotifyCraftEnd(gameObject, techType);
            ItemGoalTracker.OnConstruct(techType);
            VFXConstructing componentInChildren = gameObject.GetComponentInChildren<VFXConstructing>();
            if (componentInChildren != null)
            {
                componentInChildren.timeToConstruct = duration;
                componentInChildren.StartConstruction();
            }
            LargeWorldEntity.Register(gameObject);
            SubRoot subRoot = this.gameObject.GetComponentInParent<SubRoot>();
            VehicleDockingBay vehicleDockingBay = subRoot.GetComponentInChildren<VehicleDockingBay>();
            vehicleDockingBay.DockVehicle(gameObject.GetComponent<Vehicle>());
        }
        public void UpdateButtonActive(HashSet<TechType> techList)
        {
            gameObject.SetActive(!QMod.config.hideUnknown || KnownTech.Contains(vehicleType));
        }
        public void Start()
        {
            if(QMod.config.hideUnknown)
            {
                KnownTech.onChanged += UpdateButtonActive;
                UpdateButtonActive(null);
            }
            itemIcon.SetBackgroundSprite(SpriteManager.GetBackground(CraftData.BackgroundType.Normal));
        }
    }
}
