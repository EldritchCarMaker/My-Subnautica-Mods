using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using System;
using Logger = QModManager.Utility.Logger;
using UnityEngine;
using System.Collections;
using UWE;
using UnityEngine.EventSystems;
using System.Reflection;
using Story;
using UnityEngine.UI;
using SMLHelper.V2.Utility;
using System.IO;
using Sprite = Atlas.Sprite;

namespace CyclopsVehicleUpgradeConsole
{
    [HarmonyPatch]
    public static class MakeThing
    {

        //Todo -

        //Understand code

        //Make buttons to create vehicles from within moonpool


        [HarmonyPatch(typeof(SubRoot), nameof(SubRoot.Start))]
        [HarmonyPostfix]
        public static void Something(SubRoot __instance)
        {
            if (!__instance.isCyclops) return;

            VehicleDockingBay dockingBay = __instance.GetComponentInChildren<VehicleDockingBay>();
            CyclopsVehicleStorageTerminalManager cyclopsConsole = __instance.GetComponentInChildren<CyclopsVehicleStorageTerminalManager>();
            if (cyclopsConsole == null) return;

            //check if cyclops already has the console
            if (cyclopsConsole.GetComponentInChildren<SubNameInput>(true) != null) return;

            CoroutineHost.StartCoroutine(MakeUpgradeConsole(cyclopsConsole, dockingBay));
        }
        [HarmonyPatch(typeof(VehicleDockingBay), nameof(VehicleDockingBay.DockVehicle))]
        [HarmonyPostfix]
        public static void SetTarget(VehicleDockingBay __instance, Vehicle vehicle)
        {
            SubRoot subRoot = __instance.GetComponentInParent<SubRoot>();

            if (!subRoot.isCyclops) return;

            SubNameInput subNameInput = subRoot.GetComponentInChildren<CyclopsVehicleStorageTerminalManager>().GetComponentInChildren<SubNameInput>(true);
            subNameInput.SetTarget(vehicle.subName);
            SetActive(__instance.gameObject);
        }
        [HarmonyPatch(typeof(VehicleDockingBay), nameof(VehicleDockingBay.OnUndockingStart))]
        [HarmonyPostfix]
        public static void RemoveTarget(VehicleDockingBay __instance)
        {
            SubRoot subRoot = __instance.GetComponentInParent<SubRoot>();

            if (!subRoot.isCyclops) return;

            SubNameInput subNameInput = subRoot.GetComponentInChildren<CyclopsVehicleStorageTerminalManager>().GetComponentInChildren<SubNameInput>(true);
            subNameInput.SetTarget(null);
            SetInActive(__instance.gameObject);
        }





        public static void SetActive(GameObject gameObject)
        {
            GameObject vehicleTerminal = gameObject.GetComponentInParent<SubRoot>().transform.Find("CyclopsVehicleStorageTerminal").gameObject;

            if (vehicleTerminal == null) return;
            Logger.Log(Logger.Level.Info, "1", null, true); 

            GameObject screen = vehicleTerminal.transform.Find("GUIScreen").gameObject;
            for(var i = 0; i < screen.transform.childCount; i++)
            {
                GameObject child = screen.transform.GetChild(i).gameObject;
                if(!child.name.Equals("Swap Button"))
                {
                    Logger.Log(Logger.Level.Info, "2", null, true); 
                    child.SetActive(false);
                }
                else
                {
                    Logger.Log(Logger.Level.Info, "3", null, true); 
                    child.GetComponent<SwapButton>().colorScreenActive = false;
                }
            }
            Logger.Log(Logger.Level.Info, "4", null, true); 
            vehicleTerminal.GetComponent<CyclopsVehicleStorageTerminalManager>().OnDockedChanged();

            Logger.Log(Logger.Level.Info, "5", null, true); 
            vehicleTerminal.transform.Find("EditScreen").gameObject.GetComponent<SubNameInput>().uiActive.SetActive(false);
        }
        public static void SetInActive(GameObject gameObject)
        {
            GameObject vehicleTerminal = gameObject.GetComponentInParent<SubRoot>().transform.Find("CyclopsVehicleStorageTerminal").gameObject;

            if (vehicleTerminal == null) return;

            GameObject screen = vehicleTerminal.transform.Find("GUIScreen").gameObject;
            for (var i = 0; i < screen.transform.childCount; i++)
            {
                GameObject child = screen.transform.GetChild(i).gameObject;
                if (!child.name.Equals("Swap Button"))
                {
                    child.SetActive(false);
                }
                else
                {
                    child.GetComponent<SwapButton>().colorScreenActive = true;
                }
            }
            vehicleTerminal.transform.Find("EditScreen").gameObject.GetComponent<SubNameInput>().uiActive.SetActive(true);
        }




        public static IEnumerator MakeUpgradeConsole(CyclopsVehicleStorageTerminalManager cyclopsConsole, VehicleDockingBay dockingBay)
        {
            yield return new WaitUntil(() => Base.pieces != null && !Base.pieces[(int)Base.Piece.MoonpoolUpgradeConsole].Equals(default(Base.PieceDef)) && Base.pieces[(int)Base.Piece.MoonpoolUpgradeConsole].prefab != null);

            var prefab = Base.pieces[(int)Base.Piece.MoonpoolUpgradeConsole].prefab.gameObject;
            SubNameInput moonpoolConsole = prefab.GetComponentInChildren<SubNameInput>();
            
            GameObject cyclopsConsoleGUI = cyclopsConsole.gameObject.transform.Find("GUIScreen").gameObject;

            GameObject gameObject4 = GameObject.Instantiate(moonpoolConsole.gameObject);
            gameObject4.transform.position = cyclopsConsoleGUI.transform.position;
            gameObject4.gameObject.transform.rotation = cyclopsConsoleGUI.gameObject.transform.rotation;
            gameObject4.transform.SetParent(cyclopsConsole.gameObject.transform);
            gameObject4.name = "EditScreen";
            GameObject.Destroy(gameObject4.transform.Find("Inactive").gameObject);
            Vehicle dockedVehicle = dockingBay.GetDockedVehicle();

            gameObject4.transform.Find("Active").gameObject.SetActive(dockedVehicle != null);
            if (dockedVehicle != null)
            {
                gameObject4.GetComponent<SubNameInput>().SetTarget(dockedVehicle.subName);
            }

            /*
            GameObject buttonObject = new GameObject("Button Object");
            buttonObject.EnsureComponent<SwapButton>();
            buttonObject.transform.position = cyclopsConsoleGUI.transform.position + new Vector3(0, 0, 1);
            buttonObject.SetActive(true);
            */
            GameObject button = GameObject.Instantiate(cyclopsConsoleGUI.gameObject.transform.Find("Seamoth").Find("Modules").gameObject.GetComponent<CyclopsVehicleStorageTerminalButton>().gameObject, cyclopsConsoleGUI.gameObject.transform);

            button.transform.position += 0.765f * button.gameObject.transform.right;
            button.transform.position -= 0.05f * button.gameObject.transform.up;


            button.AddComponent<SwapButton>();

            GameObject.Destroy(button.GetComponent<CyclopsVehicleStorageTerminalButton>());
            button.name = "Swap Button";

            GameObject noVehicleScreen = cyclopsConsoleGUI.gameObject.transform.Find("NoVehicle").gameObject;
            
            noVehicleScreen.AddComponent<Canvas>();
            GameObject buttonSeamoth = GameObject.Instantiate(cyclopsConsoleGUI.gameObject.transform.Find("Seamoth").Find("Modules").gameObject.GetComponent<CyclopsVehicleStorageTerminalButton>().gameObject, noVehicleScreen.transform);
            
            buttonSeamoth.AddComponent<MakeVehicleButton>();
            GameObject.Destroy(buttonSeamoth.GetComponent<CyclopsVehicleStorageTerminalButton>());

            GameObject buttonPrawn = GameObject.Instantiate(cyclopsConsoleGUI.gameObject.transform.Find("Seamoth").Find("Modules").gameObject.GetComponent<CyclopsVehicleStorageTerminalButton>().gameObject, noVehicleScreen.transform);

            MakeVehicleButton component = buttonPrawn.AddComponent<MakeVehicleButton>();
            component.vehicleType = TechType.Exosuit;
            GameObject.Destroy(buttonPrawn.GetComponent<CyclopsVehicleStorageTerminalButton>());
        }
        public static IEnumerator MakeAndDockSeamoth(GameObject startingObject)
        {
            var task = CraftData.GetPrefabForTechTypeAsync(TechType.Seamoth);
            yield return task;
            var prefab = task.GetResult();

            GameObject seamoth = GameObject.Instantiate(prefab);

            SubRoot subRoot = startingObject.GetComponentInParent<SubRoot>();

            VehicleDockingBay vehicleDockingBay = subRoot.GetComponentInChildren<VehicleDockingBay>();
            vehicleDockingBay.DockVehicle(seamoth.GetComponent<Vehicle>());
        }
    }
    public class SwapButton : HandTarget, IHandTarget, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerClickHandler
    {
        private string hoverText = "Swap Screens";
        public bool colorScreenActive = false;
        public void OnHandClick(GUIHand hand)
        {
            if(colorScreenActive)
            {
                colorScreenActive = false;
                MakeThing.SetActive(gameObject);
            }
            else
            {
                colorScreenActive = true;
                MakeThing.SetInActive(gameObject);
            }
            if (hand == null)
            {
                Logger.Log(Logger.Level.Info, "pointer", null, true);
            }
            else
            {
                Logger.Log(Logger.Level.Info, "hand", null, true);
            }
        }

        public void OnHandHover(GUIHand hand)
        {
            if (hand.IsFreeToInteract())
            {
                HandReticle.main.SetIcon(HandReticle.IconType.Hand, 1f);
                HandReticle.main.SetInteractText(this.hoverText);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnHandClick(null);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            HandReticle.main.SetIcon(HandReticle.IconType.Hand, 1f);
            HandReticle.main.SetInteractText(this.hoverText);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            HandReticle.main.SetIcon(HandReticle.IconType.Default, 1f);
            HandReticle.main.SetInteractText("");
        }
    }
    public class MakeVehicleButton : HandTarget, IHandTarget, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerClickHandler
    {
        private string hoverText = "Make Vehicle";
        public TechType vehicleType = TechType.Seamoth;
        public bool colorScreenActive = false;
        readonly string AssetsFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");
        public void OnHandClick(GUIHand hand)
        {
            if(hand == null)
            {
                Logger.Log(Logger.Level.Info, "pointer", null, true);
            }
            else
            {
                Logger.Log(Logger.Level.Info, "hand", null, true); 
            }
            CoroutineHost.StartCoroutine(OnCraftingBegin(this.vehicleType, 5f));
        }
        public IEnumerator OnCraftingBegin(TechType techType, float duration)
        {
            Vector3 zero = Vector3.zero;
            Quaternion identity = Quaternion.identity;

            //this.GetCraftTransform(techType, ref zero, ref identity);
            

            GameObject gameObject;
            if (techType == TechType.Cyclops)
            {
                SubConsoleCommand.main.SpawnSub("cyclops", zero, identity);
                FMODUWE.PlayOneShot("event:/tools/constructor/spawn", zero, 1f);
                gameObject = SubConsoleCommand.main.GetLastCreatedSub();
            }
            else
            {
                var task = CraftData.GetPrefabForTechTypeAsync(techType);
                yield return task;
                var prefab = task.GetResult();

                gameObject = GameObject.Instantiate(prefab);
                Transform component = gameObject.GetComponent<Transform>();
                component.position = zero;
                component.rotation = identity;
            }
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
        public override void Awake()
        {
            Logger.Log(Logger.Level.Info, "WooWee! I'm Awake!", null, true);
            try
            {
                hoverText = "Make " + vehicleType;
                Sprite sprite = ImageUtils.LoadSpriteFromFile(Path.Combine(AssetsFolder, "PageChangerBackground.png"));
                gameObject.GetComponent<Image>().sprite = null;
                Logger.Log(Logger.Level.Info, "Still Alive bitches", null, true);
            }catch(Exception e)
            {
                Logger.Log(Logger.Level.Info, "Death and Dishoner await me", null, true); 
            }
        }
        public void OnHandHover(GUIHand hand)
        {
            if (hand.IsFreeToInteract())
            {
                HandReticle.main.SetIcon(HandReticle.IconType.Hand, 1f);
                HandReticle.main.SetInteractText(this.hoverText);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnHandClick(null);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            HandReticle.main.SetIcon(HandReticle.IconType.Hand, 1f);
            HandReticle.main.SetInteractText(this.hoverText);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            HandReticle.main.SetIcon(HandReticle.IconType.Default, 1f);
            HandReticle.main.SetInteractText("");
        }
    }
}

/*
 * button.gameObject.transform.position -= 0.75f * button.gameObject.transform.right;
GameObject button = GameObject.Instantiate<GameObject>(sawp.gameObject, noVehicle.gameObject.transform);

vehicleDockingBay.DockVehicle(seamoth.GetComponent<Vehicle>());
GameObject seamoth = GameObject.Instantiate(CraftData.GetPrefabForTechType(TechType.Seamoth));

protected static void OnCraftingBegin(TechType techType, float duration)
{
	Vector3 zero = Vector3.zero;
	Quaternion identity = Quaternion.identity;
	this.GetCraftTransform(techType, ref zero, ref identity);
	if (!GameInput.GetButtonHeld(GameInput.Button.Sprint))
	{
		uGUI.main.craftingMenu.Close(this);
		this.cinematicController.DisengageConstructor();
	}
	GameObject gameObject;
	if (techType == TechType.Cyclops)
	{
		SubConsoleCommand.main.SpawnSub("cyclops", zero, identity);
		FMODUWE.PlayOneShot("event:/tools/constructor/spawn", zero, 1f);
		gameObject = SubConsoleCommand.main.GetLastCreatedSub();
	}
	else
	{
		gameObject = CraftData.InstantiateFromPrefab(techType, false);
		Transform component = gameObject.GetComponent<Transform>();
		component.position = zero;
		component.rotation = identity;
	}
	CrafterLogic.NotifyCraftEnd(gameObject, techType);
	ItemGoalTracker.OnConstruct(techType);
	VFXConstructing componentInChildren = gameObject.GetComponentInChildren<VFXConstructing>();
	if (componentInChildren != null)
	{
		componentInChildren.timeToConstruct = duration;
		componentInChildren.StartConstruction();
	}
	if (gameObject.GetComponentInChildren<BuildBotPath>() == null)
	{
		new GameObject("ConstructorBeam").AddComponent<TwoPointLine>().Initialize(this.beamMaterial, base.transform, gameObject.transform, 0.1f, 1f, duration);
	}
	else
	{
		this.constructor.SendBuildBots(gameObject);
	}
	LargeWorldEntity.Register(gameObject);
}

*/