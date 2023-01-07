using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using System;
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
using static CyclopsVehicleUpgradeConsole.VehicleConsoleCreation;

namespace CyclopsVehicleUpgradeConsole
{
    public static class MakeThing
    {

        //Todo -

        //Understand code

        
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
    
    
}
/*much of this should be completely unnecessary, however, it is what I got to work so I'm putting it here until I can weed through it
 * Welcome to C# REPL (read-evaluate-print loop)! Enter "help" to get a list of common methods.
> var collision = geti<UnityEngine.GameObject>()
> collision.AddComponent<FCS_EnergySolutions.Mods.WindSurfer.Mono.ScreenTrigger>()
Collision (FCS_EnergySolutions.Mods.WindSurfer.Mono.ScreenTrigger)
> var ST = geti<FCS_EnergySolutions.Mods.WindSurfer.Mono.ScreenTrigger>()
> ST.Select(true);
> ST.Select(true);
> ST.Select(true);
> ST.Select(true);
> ST.Select(true);
> ST.Select(true);
> ST.Select(true);
> var II = geti<uGUI_ItemIcon>()
> II.OnPointerClick(null);
System.NullReferenceException: Object reference not set to an instance of an object
  at uGUI_ItemIcon.OnPointerClick (UnityEngine.EventSystems.PointerEventData eventData) [0x00002] in <b20a3a5ada624fe2a1501d935cca65f7>:0 
  at <InteractiveExpressionClass 86>.Host86 (System.Object& $retval) [0x00000] in <dcb618031b4d4f83849df5f6251085ce>:0 
  at RuntimeUnityEditor.Core.REPL.ReplWindow.Evaluate (System.String str) [0x00018] in <446556a8b71f444eba6db50afacc710e>:0 
> var manager = geti<uGUI_CraftNode>()
> manager is uGUI_CraftNode
True
> var CraftManager = manager as uGUI_CraftNode;
> var MyItemIcon = new uGUI_ItemIcon(0;
(1,37): error CS1525: Unexpected symbol `;', expecting `)' or `,'
> var MyItemIcon = new uGUI_ItemIcon();
(1,2): error CS1039: Unterminated string literal
> var MyCraftNode = new uGUI_CraftNode(uGUI.main.craftingMenu, "Hello", 1, TreeAction.Craft, TechType.Aerogel);
> MyItemIcon.manager = MyCraftNode;
> MyCraftNode != null
True
> MyItemIcon != null
False
> MyItemIcon != null
False
> var MyItemIcon = new uGUI_ItemIcon();
> MyItemIcon != null
False
> MyItemIcon.manager = MyCraftNode;
> var MyItemIcon = collision.AddComponent<uGUI_ItemIcon>();
> MyItemIcon.manager = MyCraftNode;
> MyItemIcon != null
True
> MyCraftNode != null
True
> var resourcesObj = geti<UnityEngine.GameObject>()
> var ResourcesObjClone = GameObject.Instantiate(resourcesObj);
> MyItemIcon = ResourcesObjClone.AddComponent<uGUI_ItemIcon>();
> MyItemIcon.manager = MyCraftNode;
> ResourcesObjClone.transform.position = resourcesObj.transform.position;
> ResourcesObjClone.transform.parent = resourcesObj.transform.parent;
> ResourcesObjClone.transform.position = resourcesObj.transform.position;
> var resourcesObj = geti<UnityEngine.GameObject>()
> var ResourcesObjClone = GameObject.Instantiate(resourcesObj);
> MyItemIcon = ResourcesObjClone.AddComponent<uGUI_ItemIcon>();
> MyItemIcon.manager = MyCraftNode;
System.NullReferenceException: Object reference not set to an instance of an object
  at <InteractiveExpressionClass 606>.Host606 (System.Object& $retval) [0x00000] in <5f9d4224ec634100b9fb69f1824d2b0f>:0 
  at RuntimeUnityEditor.Core.REPL.ReplWindow.Evaluate (System.String str) [0x00018] in <446556a8b71f444eba6db50afacc710e>:0 
> var resourcesObj = geti<UnityEngine.GameObject>()
> var ResourcesObjClone = GameObject.Instantiate(resourcesObj);
> MyItemIcon = ResourcesObjClone.AddComponent<uGUI_ItemIcon>();
> MyItemIcon.manager = MyCraftNode;
> ResourcesObjClone.transform.parent = resourcesObj.transform.parent;
> ResourcesObjClone.transform.localScale = resourcesObj.transform.localScale;
> ResourcesObjClone.transform.position = resourcesObj.transform.position;
> MyItemIcon.CreateBackground();
> MyItemIcon.CreateForeground();
> MyItemIcon.foreground.sprite = SpriteManager.Get(TechType.Seamoth);
> var collision = geti<UnityEngine.GameObject>()
> collision.AddComponent<Canvas>();
Collision (UnityEngine.Canvas)
> ResourcesObjClone.transform.parent = collision.transform;
> ResourcesObjClone.transform.position = resourcesObj.transform.position;
(1,3): error CS0103: The name `P' does not exist in the current context
(1,3): error CS0103: The name `Pl' does not exist in the current context
(1,3): error CS0103: The name `Pla' does not exist in the current context
(1,3): error CS0103: The name `Play' does not exist in the current context
(1,3): error CS0103: The name `Playe' does not exist in the current context
(1,10): error CS0120: An object reference is required to access non-static member `UnityEngine.Component.transform'
(1,10): error CS0117: `Player' does not contain a definition for `m'
(1,10): error CS0117: `Player' does not contain a definition for `ma'
(1,10): error CS0117: `Player' does not contain a definition for `mai'
> ResourcesObjClone.transform.position = Player.main.transform.position;
> ResourcesObjClone.transform.position = Player.main.transform.position;
> MyItemIcon.foreground.sprite = SpriteManager.Get(TechType.Seamoth);
> collision.AddComponent<uGUI_GraphicRaycaster>();
Name: Collision (UnityEngine.GameObject)
eventCamera: MainCamera (UnityEngine.Camera)
sortOrderPriority: -2147483648
renderOrderPriority: -2147483648
> ResourcesObjClone.transform.position = Player.main.transform.position;
> collision.AddComponent<CanvasGroup>();
Collision (UnityEngine.CanvasGroup)
> collision.AddComponent<uGUI_GraphicScaler>();
(1,25): error CS0246: The type or namespace name `uGUI_GraphicScaler' could not be found. Are you missing an assembly reference?
> collision.AddComponent<uGUI_CanvasScaler>();
Collision (uGUI_CanvasScaler)
> ResourcesObjClone.transform.position = Player.main.transform.position;
> ResourcesObjClone.transform.position = Player.main.transform.position;
> ResourcesObjClone.transform.localScale = new Vector3(1, 1, 1);
> ResourcesObjClone.transform.eulerAngles += new Vector3(0, 90, 0);
> ResourcesObjClone.transform.eulerAngles += new Vector3(0, 90, 0);
> MyCraftNode.techType0 = TechType.Seamoth();
(1,35): error CS0122: `TechType.Seamoth' is inaccessible due to its protection level
(1,35): error CS0119: Expression denotes a `value', where a `method group' was expected
> MyCraftNode.techType0 = TechType.Seamoth;

# History of reed commands:
MyCraftNode != null
MyItemIcon != null
MyItemIcon != null
var MyItemIcon = new uGUI_ItemIcon();
MyItemIcon != null
MyItemIcon.manager = MyCraftNode;
var MyItemIcon = collision.AddComponent<uGUI_ItemIcon>();
MyItemIcon.manager = MyCraftNode;
MyItemIcon != null
MyCraftNode != null
var resourcesObj = geti()
var ResourcesObjClone = GameObject.Instantiate(resourcesObj);
MyItemIcon = ResourcesObjClone.AddComponent<uGUI_ItemIcon>();
MyItemIcon.manager = MyCraftNode;
ResourcesObjClone.transform.position = resourcesObj.transform.position;
ResourcesObjClone.transform.parent = resourcesObj.transform.parent;
ResourcesObjClone.transform.position = resourcesObj.transform.position;
var resourcesObj = geti()
var ResourcesObjClone = GameObject.Instantiate(resourcesObj);
MyItemIcon = ResourcesObjClone.AddComponent<uGUI_ItemIcon>();
MyItemIcon.manager = MyCraftNode;
var resourcesObj = geti()
var ResourcesObjClone = GameObject.Instantiate(resourcesObj);
MyItemIcon = ResourcesObjClone.AddComponent<uGUI_ItemIcon>();
MyItemIcon.manager = MyCraftNode;
ResourcesObjClone.transform.parent = resourcesObj.transform.parent;
ResourcesObjClone.transform.localScale = resourcesObj.transform.localScale;
ResourcesObjClone.transform.position = resourcesObj.transform.position;
MyItemIcon.CreateBackground();
MyItemIcon.CreateForeground();
MyItemIcon.foreground.sprite = SpriteManager.Get(TechType.Seamoth);
var collision = geti()
collision.AddComponent<Canvas>();
ResourcesObjClone.transform.parent = collision.transform;
ResourcesObjClone.transform.position = resourcesObj.transform.position;
ResourcesObjClone.transform.position = Player.main.transform.position;
ResourcesObjClone.transform.position = Player.main.transform.position;
MyItemIcon.foreground.sprite = SpriteManager.Get(TechType.Seamoth);
collision.AddComponent<uGUI_GraphicRaycaster>();
ResourcesObjClone.transform.position = Player.main.transform.position;
collision.AddComponent<CanvasGroup>();
collision.AddComponent<uGUI_GraphicScaler>();
collision.AddComponent<uGUI_CanvasScaler>();
ResourcesObjClone.transform.position = Player.main.transform.position;
ResourcesObjClone.transform.position = Player.main.transform.position;
ResourcesObjClone.transform.localScale = new Vector3(1, 1, 1);
ResourcesObjClone.transform.eulerAngles += new Vector3(0, 90, 0);
ResourcesObjClone.transform.eulerAngles += new Vector3(0, 90, 0);
MyCraftNode.techType0 = TechType.Seamoth();
MyCraftNode.techType0 = TechType.Seamoth;

# History of reed commands:
MyCraftNode != null
MyItemIcon != null
MyItemIcon != null
var MyItemIcon = new uGUI_ItemIcon();
MyItemIcon != null
MyItemIcon.manager = MyCraftNode;
var MyItemIcon = collision.AddComponent<uGUI_ItemIcon>();
MyItemIcon.manager = MyCraftNode;
MyItemIcon != null
MyCraftNode != null
var resourcesObj = geti()
var ResourcesObjClone = GameObject.Instantiate(resourcesObj);
MyItemIcon = ResourcesObjClone.AddComponent<uGUI_ItemIcon>();
MyItemIcon.manager = MyCraftNode;
ResourcesObjClone.transform.position = resourcesObj.transform.position;
ResourcesObjClone.transform.parent = resourcesObj.transform.parent;
ResourcesObjClone.transform.position = resourcesObj.transform.position;
var resourcesObj = geti()
var ResourcesObjClone = GameObject.Instantiate(resourcesObj);
MyItemIcon = ResourcesObjClone.AddComponent<uGUI_ItemIcon>();
MyItemIcon.manager = MyCraftNode;
var resourcesObj = geti()
var ResourcesObjClone = GameObject.Instantiate(resourcesObj);
MyItemIcon = ResourcesObjClone.AddComponent<uGUI_ItemIcon>();
MyItemIcon.manager = MyCraftNode;
ResourcesObjClone.transform.parent = resourcesObj.transform.parent;
ResourcesObjClone.transform.localScale = resourcesObj.transform.localScale;
ResourcesObjClone.transform.position = resourcesObj.transform.position;
MyItemIcon.CreateBackground();
MyItemIcon.CreateForeground();
MyItemIcon.foreground.sprite = SpriteManager.Get(TechType.Seamoth);
var collision = geti()
collision.AddComponent<Canvas>();
ResourcesObjClone.transform.parent = collision.transform;
ResourcesObjClone.transform.position = resourcesObj.transform.position;
ResourcesObjClone.transform.position = Player.main.transform.position;
ResourcesObjClone.transform.position = Player.main.transform.position;
MyItemIcon.foreground.sprite = SpriteManager.Get(TechType.Seamoth);
collision.AddComponent<uGUI_GraphicRaycaster>();
ResourcesObjClone.transform.position = Player.main.transform.position;
collision.AddComponent<CanvasGroup>();
collision.AddComponent<uGUI_GraphicScaler>();
collision.AddComponent<uGUI_CanvasScaler>();
ResourcesObjClone.transform.position = Player.main.transform.position;
ResourcesObjClone.transform.position = Player.main.transform.position;
ResourcesObjClone.transform.localScale = new Vector3(1, 1, 1);
ResourcesObjClone.transform.eulerAngles += new Vector3(0, 90, 0);
ResourcesObjClone.transform.eulerAngles += new Vector3(0, 90, 0);
MyCraftNode.techType0 = TechType.Seamoth();
MyCraftNode.techType0 = TechType.Seamoth;

# History of reed commands:
MyCraftNode != null
MyItemIcon != null
MyItemIcon != null
var MyItemIcon = new uGUI_ItemIcon();
MyItemIcon != null
MyItemIcon.manager = MyCraftNode;
var MyItemIcon = collision.AddComponent<uGUI_ItemIcon>();
MyItemIcon.manager = MyCraftNode;
MyItemIcon != null
MyCraftNode != null
var resourcesObj = geti()
var ResourcesObjClone = GameObject.Instantiate(resourcesObj);
MyItemIcon = ResourcesObjClone.AddComponent<uGUI_ItemIcon>();
MyItemIcon.manager = MyCraftNode;
ResourcesObjClone.transform.position = resourcesObj.transform.position;
ResourcesObjClone.transform.parent = resourcesObj.transform.parent;
ResourcesObjClone.transform.position = resourcesObj.transform.position;
var resourcesObj = geti()
var ResourcesObjClone = GameObject.Instantiate(resourcesObj);
MyItemIcon = ResourcesObjClone.AddComponent<uGUI_ItemIcon>();
MyItemIcon.manager = MyCraftNode;
var resourcesObj = geti()
var ResourcesObjClone = GameObject.Instantiate(resourcesObj);
MyItemIcon = ResourcesObjClone.AddComponent<uGUI_ItemIcon>();
MyItemIcon.manager = MyCraftNode;
ResourcesObjClone.transform.parent = resourcesObj.transform.parent;
ResourcesObjClone.transform.localScale = resourcesObj.transform.localScale;
ResourcesObjClone.transform.position = resourcesObj.transform.position;
MyItemIcon.CreateBackground();
MyItemIcon.CreateForeground();
MyItemIcon.foreground.sprite = SpriteManager.Get(TechType.Seamoth);
var collision = geti()
collision.AddComponent<Canvas>();
ResourcesObjClone.transform.parent = collision.transform;
ResourcesObjClone.transform.position = resourcesObj.transform.position;
ResourcesObjClone.transform.position = Player.main.transform.position;
ResourcesObjClone.transform.position = Player.main.transform.position;
MyItemIcon.foreground.sprite = SpriteManager.Get(TechType.Seamoth);
collision.AddComponent<uGUI_GraphicRaycaster>();
ResourcesObjClone.transform.position = Player.main.transform.position;
collision.AddComponent<CanvasGroup>();
collision.AddComponent<uGUI_GraphicScaler>();
collision.AddComponent<uGUI_CanvasScaler>();
ResourcesObjClone.transform.position = Player.main.transform.position;
ResourcesObjClone.transform.position = Player.main.transform.position;
ResourcesObjClone.transform.localScale = new Vector3(1, 1, 1);
ResourcesObjClone.transform.eulerAngles += new Vector3(0, 90, 0);
ResourcesObjClone.transform.eulerAngles += new Vector3(0, 90, 0);
MyCraftNode.techType0 = TechType.Seamoth();
MyCraftNode.techType0 = TechType.Seamoth;
(1,1): error CS1039: Unterminated string literal
> var testobj = new GameObject("TestingsShit");
> testobj.transform.parent = ResourcesObjClone.transform.parent;
> testobj.transform.position = Player.main.transform.position;
> testobj.AddComponent<uGUI_Icon>();
TestingsShit (uGUI_Icon)
> testobj.transform.localScale = new Vector3(1, 1, 1);
> testobj.AddComponent<uGUI_ItemIcon>();
> testobj.AddComponent<uGUI_ItemIcon>();
TestingsShit (uGUI_ItemIcon)
> testobj.transform.localScale = new Vector3(1, 1, 1);
> testobj.transform.localScale = new Vector3(1, 1, 1);
> testobj.transform.position = Player.main.transform.position;
> testobj.transform.position = Player.main.transform.position;
> var itemicon = geti<uGUI_ItemIcon>()
> itemicon.foreground.sprite = SpriteManager.Get(TechType.Seamoth);
> itemicon.background.sprite = SpriteManager.GetBackground(CraftData.BackgroundType.Normal;
(1,89): error CS1525: Unexpected symbol `;', expecting `)' or `,'
> itemicon.background.sprite = SpriteManager.GetBackground(CraftData.BackgroundType.Normal);
> itemicon.background.sprite = SpriteManager.GetBackground(CraftData.BackgroundType.Blueprint);
> itemicon.background.sprite = SpriteManager.GetBackground(TechType.Seamoth);
> itemicon.manager = MyCraftNode;

*/