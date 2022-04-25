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

            var manager = vehicleTerminal.GetComponent<CyclopsVehicleStorageTerminalManager>();
            GameObject screen = vehicleTerminal.transform.Find("GUIScreen").gameObject;
            for(var i = 0; i < screen.transform.childCount; i++)
            {
                GameObject child = screen.transform.GetChild(i).gameObject;
                if (child.name.Equals("SwapButton"))
                {
                    child.GetComponent<SwapButton>().colorScreenActive = false;
                }
                else if (child.name.Equals("MakeSeamothButton") && manager.currentVehicle == null)
                {
                    child.SetActive(true);
                }
                else if (child.name.Equals("MakeExoSuitButton") && manager.currentVehicle == null)
                {
                    child.SetActive(true);
                }
                else
                {
                    child.SetActive(false);
                }
            }
            
            manager.OnDockedChanged();


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
                if(child.name.Equals("SwapButton"))
                {
                    child.GetComponent<SwapButton>().colorScreenActive = true;
                }
                else
                {
                    child.SetActive(false);
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
            button.name = "SwapButton";

            GameObject noVehicleScreen = cyclopsConsoleGUI.gameObject.transform.Find("NoVehicle").gameObject;
            noVehicleScreen.transform.Find("XIcon").gameObject.SetActive(false);

            GameObject buttonSeamoth = GameObject.Instantiate(cyclopsConsoleGUI.gameObject.transform.Find("Seamoth").Find("Modules").gameObject.GetComponent<CyclopsVehicleStorageTerminalButton>().gameObject, cyclopsConsoleGUI.gameObject.transform);
            
            buttonSeamoth.AddComponent<MakeVehicleButton>();
            GameObject.Destroy(buttonSeamoth.GetComponent<CyclopsVehicleStorageTerminalButton>());
            buttonSeamoth.name = "MakeSeamothButton";

            buttonSeamoth.transform.position += 0.15f * button.gameObject.transform.right;
            buttonSeamoth.transform.position -= 0.1f * button.gameObject.transform.up;

            GameObject buttonPrawn = GameObject.Instantiate(cyclopsConsoleGUI.gameObject.transform.Find("Seamoth").Find("Modules").gameObject.GetComponent<CyclopsVehicleStorageTerminalButton>().gameObject, cyclopsConsoleGUI.gameObject.transform);

            MakeVehicleButton component = buttonPrawn.AddComponent<MakeVehicleButton>();
            component.vehicleType = TechType.Exosuit;
            GameObject.Destroy(buttonPrawn.GetComponent<CyclopsVehicleStorageTerminalButton>());
            buttonPrawn.name = "MakeExoSuitButton";

            buttonPrawn.transform.position += 0.38f * button.gameObject.transform.right;
            buttonPrawn.transform.position -= 0.1f * button.gameObject.transform.up;

            //make text
            GameObject text = GameObject.Instantiate(noVehicleScreen.transform.Find("Text").gameObject, noVehicleScreen.transform);
            text.GetComponent<Text>().text = "Fabricate Vehicle In Empty Bay";
            text.transform.localScale = new Vector3(1, 1, 1);
            text.transform.position -= 0.14f * text.transform.up;

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
        readonly string AssetsFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");
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
        }
        public override void Awake()
        {
            Atlas.Sprite myAtlas = ImageUtils.LoadSpriteFromFile(Path.Combine(AssetsFolder, "PageChangerBackground.png"));
            var texture = myAtlas.texture;
            var sprite = UnityEngine.Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), Vector2.one * 0.5f);
            gameObject.transform.GetChild(0).GetComponent<Image>().sprite = sprite;
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
        public void OnHandClick(GUIHand hand)
        {
            SubRoot subRoot = this.gameObject.GetComponentInParent<SubRoot>();
            VehicleDockingBay vehicleDockingBay = subRoot.GetComponentInChildren<VehicleDockingBay>();
            if(vehicleDockingBay.dockedVehicle != null)
            {
                ErrorMessage.AddMessage("There is already a vehicle docked!");
                MakeThing.SetActive(gameObject);
                return;
            }
            if (!CrafterLogic.IsCraftRecipeUnlocked(this.vehicleType)) 
            {
                ErrorMessage.AddMessage("You haven't unlocked the blueprint for this vehicle");
                return;
            }
            if (!CrafterLogic.ConsumeResources(this.vehicleType))
            {
                return;
            }
            CoroutineHost.StartCoroutine(OnCraftingBegin(this.vehicleType, 5f));
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
        public override void Awake()
        {
            hoverText = "Make " + vehicleType;

            /*
            Atlas.Sprite myAtlas = ImageUtils.LoadSpriteFromFile(Path.Combine(AssetsFolder, "PageChangerBackground.png"));
            var texture = myAtlas.texture;
            var sprite = UnityEngine.Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), Vector2.one * 0.5f);
            gameObject.transform.GetChild(0).GetComponent<Image>().sprite = sprite;
            */
            gameObject.transform.GetChild(0).GetComponent<Image>().sprite = GetSprite(this.vehicleType);
        }
        private static UnityEngine.Sprite GetSprite(TechType item)
        {
            // Gets the atlas (group of sprites) for item icons
            Atlas itemAtlas = Atlas.GetAtlas("Items");
            if (itemAtlas != null)
            {
                // finds the serial data corresponding to the item and returns the sprite of it if successful.
                var itemData = itemAtlas.serialData.Find(x => x.name == item.AsString(true));
                if (itemData != null) return itemData.sprite;
            }

            return null;
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