using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using CyclopsVehicleUpgradeConsole.Monobehaviours;
using UnityEngine.UI;
using Logger = QModManager.Utility.Logger;

namespace CyclopsVehicleUpgradeConsole
{
    public class VehicleConsoleCreation
    {
        public static void SetActive(GameObject gameObject)
        {
            GameObject vehicleTerminal = gameObject.GetComponentInParent<SubRoot>().transform.Find("CyclopsVehicleStorageTerminal").gameObject;

            if (vehicleTerminal == null) return;

            var manager = vehicleTerminal.GetComponent<CyclopsVehicleStorageTerminalManager>();
            GameObject screen = vehicleTerminal.transform.Find("GUIScreen").gameObject;
            for (var i = 0; i < screen.transform.childCount; i++)
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
                else if (child.name.Equals("MakePrawnButton") && manager.currentVehicle == null)
                {
                    child.SetActive(true);
                }
                else if (child.name.Equals("WindowButton"))
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
                if (child.name.Equals("SwapButton"))
                {
                    child.GetComponent<SwapButton>().colorScreenActive = true;
                }
                else if (child.name.Equals("WindowButton"))
                {
                    child.SetActive(true);
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


            yield return new WaitUntil(() => uGUI.main != null && uGUI.main.craftingMenu != null);
            //Old_MakeVehicleButtons(cyclopsConsoleGUI.gameObject);

            MakeSwapButton(cyclopsConsoleGUI.gameObject);
            New_MakeVehicleButtons(cyclopsConsoleGUI.gameObject);


            //make text
            GameObject noVehicleScreen = cyclopsConsoleGUI.gameObject.transform.Find("NoVehicle").gameObject;

            GameObject text = GameObject.Instantiate(noVehicleScreen.transform.Find("Text").gameObject, noVehicleScreen.transform);
            text.GetComponent<Text>().text = "Fabricate Vehicle In Empty Bay";
            text.transform.localScale = new Vector3(1, 1, 1);
            text.transform.position -= 0.14f * text.transform.up;

        }
        public static void MakeSwapButton(GameObject consoleGUIObject)
        {
            GameObject button = GameObject.Instantiate(consoleGUIObject.transform.Find("Seamoth").Find("Modules").gameObject.GetComponent<CyclopsVehicleStorageTerminalButton>().gameObject, consoleGUIObject.transform);

            button.transform.position += 0.765f * button.gameObject.transform.right;
            button.transform.position -= 0.05f * button.gameObject.transform.up;

            button.AddComponent<SwapButton>();

            GameObject.Destroy(button.GetComponent<CyclopsVehicleStorageTerminalButton>());
            button.name = "SwapButton";

            GameObject noVehicleScreen = consoleGUIObject.transform.Find("NoVehicle").gameObject;
            noVehicleScreen.transform.Find("XIcon").gameObject.SetActive(false);
        }

        public static void New_MakeVehicleButtons(GameObject consoleGUIObject)
        {
            GameObject originalButton = consoleGUIObject.transform.Find("Seamoth").Find("Modules").gameObject.GetComponent<CyclopsVehicleStorageTerminalButton>().gameObject;

            GameObject buttonObj = new GameObject("MakeSeamothButton");
            buttonObj.transform.parent = consoleGUIObject.transform;
            buttonObj.transform.rotation = originalButton.transform.rotation;

            buttonObj.transform.position = originalButton.transform.position;
            buttonObj.transform.localPosition = originalButton.transform.localPosition;
            buttonObj.transform.position -= 0.15f * originalButton.transform.right;
            buttonObj.transform.position += 0.1f * originalButton.transform.up;
            buttonObj.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);

            buttonObj.AddComponent<MakeVehicleButton>();

            var myItemIcon = buttonObj.AddComponent<uGUI_ItemIcon>();
            var myCraftNode = new uGUI_CraftNode(uGUI.main.craftingMenu, "MakeSeamoth", 1, TreeAction.Craft, TechType.Seamoth);
            myCraftNode.icon = myItemIcon;
            myItemIcon.manager = myCraftNode;
            myItemIcon.CreateBackground();
            myItemIcon.CreateForeground();
            myItemIcon.foreground.sprite = SpriteManager.Get(TechType.Seamoth);

            myItemIcon.foreground.enabled = true;

            myItemIcon.background.enabled = true;



            GameObject prawnButtonObj = new GameObject("MakePrawnButton");
            prawnButtonObj.transform.parent = consoleGUIObject.transform;
            prawnButtonObj.transform.rotation = originalButton.transform.rotation;

            prawnButtonObj.transform.position = originalButton.transform.position;
            prawnButtonObj.transform.localPosition = originalButton.transform.localPosition;
            prawnButtonObj.transform.position += 0.13f * originalButton.transform.right;
            prawnButtonObj.transform.position += 0.1f * originalButton.transform.up;
            prawnButtonObj.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);

            var MVB = prawnButtonObj.AddComponent<MakeVehicleButton>();
            MVB.vehicleType = TechType.Exosuit;

            var myItemIconPrawn = prawnButtonObj.AddComponent<uGUI_ItemIcon>();
            var myCraftNodePrawn = new uGUI_CraftNode(uGUI.main.craftingMenu, "MakePrawn", 1, TreeAction.Craft, TechType.Exosuit);
            myCraftNodePrawn.icon = myItemIconPrawn;
            myItemIconPrawn.manager = myCraftNodePrawn;
            myItemIconPrawn.CreateBackground();
            myItemIconPrawn.CreateForeground();
            myItemIconPrawn.foreground.sprite = SpriteManager.Get(TechType.Exosuit);

            myItemIconPrawn.foreground.enabled = true;

            myItemIconPrawn.background.enabled = true;
        }



        //hoping to replace, but keeping as a working version for backup because I'm paranoid I'll break it all
        public static void Old_MakeVehicleButtons(GameObject consoleGUIObject)
        {

            GameObject button = GameObject.Instantiate(consoleGUIObject.transform.Find("Seamoth").Find("Modules").gameObject.GetComponent<CyclopsVehicleStorageTerminalButton>().gameObject, consoleGUIObject.transform);

            button.transform.position += 0.765f * button.gameObject.transform.right;
            button.transform.position -= 0.05f * button.gameObject.transform.up;


            button.AddComponent<SwapButton>();

            GameObject.Destroy(button.GetComponent<CyclopsVehicleStorageTerminalButton>());
            button.name = "SwapButton";

            GameObject noVehicleScreen = consoleGUIObject.transform.Find("NoVehicle").gameObject;
            noVehicleScreen.transform.Find("XIcon").gameObject.SetActive(false);

            GameObject buttonSeamoth = GameObject.Instantiate(consoleGUIObject.transform.Find("Seamoth").Find("Modules").gameObject.GetComponent<CyclopsVehicleStorageTerminalButton>().gameObject, consoleGUIObject.transform);

            buttonSeamoth.AddComponent<MakeVehicleButton>();
            GameObject.Destroy(buttonSeamoth.GetComponent<CyclopsVehicleStorageTerminalButton>());
            buttonSeamoth.name = "MakeSeamothButton";

            buttonSeamoth.transform.position += 0.15f * button.gameObject.transform.right;
            buttonSeamoth.transform.position -= 0.1f * button.gameObject.transform.up;

            GameObject buttonPrawn = GameObject.Instantiate(consoleGUIObject.transform.Find("Seamoth").Find("Modules").gameObject.GetComponent<CyclopsVehicleStorageTerminalButton>().gameObject, consoleGUIObject.transform);

            MakeVehicleButton component = buttonPrawn.AddComponent<MakeVehicleButton>();
            component.vehicleType = TechType.Exosuit;
            GameObject.Destroy(buttonPrawn.GetComponent<CyclopsVehicleStorageTerminalButton>());
            buttonPrawn.name = "MakeExoSuitButton";

            buttonPrawn.transform.position += 0.38f * button.gameObject.transform.right;
            buttonPrawn.transform.position -= 0.1f * button.gameObject.transform.up;
        }
    }
}
