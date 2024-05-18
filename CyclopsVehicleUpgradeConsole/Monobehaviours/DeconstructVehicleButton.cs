using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#if !SN2
using Logger = QModManager.Utility.Logger;
using SMLHelper.V2.Utility;
#else
using Nautilus.Utility;
#endif

namespace CyclopsVehicleUpgradeConsole.Monobehaviours
{
    public class DeconstructVehicleButton : HandTarget, IHandTarget, IEventSystemHandler, IPointerClickHandler, IPointerHoverHandler
    {
        public const float doubleClickTimeWindow = 1.5f;
        readonly string AssetsFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");

        public float timeLastClick = 0;
        public VehicleDockingBay dockingBay;

        public override void Awake()
        {
            base.Awake();

            SubRoot subRoot = gameObject.GetComponentInParent<SubRoot>();
            dockingBay = subRoot.GetComponentInChildren<VehicleDockingBay>();

            Atlas.Sprite myAtlas = ImageUtils.LoadSpriteFromFile(Path.Combine(AssetsFolder, "VehicleDeconIcon.png"));
            var texture = myAtlas.texture;
            var sprite = UnityEngine.Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), Vector2.one * 0.5f);
            gameObject.transform.GetChild(0).GetComponent<Image>().sprite = sprite;
        }
        public void OnHandClick(GUIHand hand)
        {
            if (dockingBay == null || dockingBay.dockedVehicle == null) return;//safety check

            if(timeLastClick + doubleClickTimeWindow < Time.time)//make sure player really wants to destroy vehicle
            {
                ErrorMessage.AddMessage("Click button again to confirm deconstruction");
                timeLastClick = Time.time;
                return;
            }

            timeLastClick = Time.time;

            if (dockingBay.dockedVehicle is Exosuit suit)//can't do much with just the interface
            {
                if (suit.storageContainer.container.count > 0)//check if they're empty
                {
                    ErrorMessage.AddMessage("Vehicle storage isn't empty, can't deconstruct vehicle.");
                    return;
                }
            }


            foreach (var slot in dockingBay.dockedVehicle.modules.equipment)
            {
                if(slot.Value != null)//checks for if any slot has an equipped upgrade module
                {
                    ErrorMessage.AddMessage("Vehicle modules aren't empty, can't deconstruct vehicle.");
                    return;
                }
            }

            TechType vehicleType = //gets techtype
                dockingBay.dockedVehicle is SeaMoth ? TechType.Seamoth //is seamoth
                : dockingBay.dockedVehicle is Exosuit ? TechType.Exosuit //is prawn
                : CraftData.GetTechType(dockingBay.dockedVehicle.gameObject);//is something else, possibly modded vehicle not currently made, get techtype through this instead


            List<IIngredient> ingredients = new List<IIngredient>();

            ITechData data = CraftData.Get(vehicleType);
            for(var i = 0; i < data.ingredientCount; i++)
            {
                ingredients.Add(data.GetIngredient(i));//get every ingredient
            }

            foreach(IIngredient ingredient in ingredients)//you ever see a word so much it doesn't even look like a real word? Ingredient looks so weird now
            {
#if SN1
                for(var j = 0; j < ingredient.amount; j++)
                {
                    var newObj = CraftData.InstantiateFromPrefab(ingredient.techType);
                    if(newObj == null)
                    {
                        Logger.Log(Logger.Level.Error, $"Couldn't find prefab for techtype: {ingredient.techType}, failed to add resource to inventory", null, true);
                        continue;
                    }

                    var pickup = newObj.GetComponent<Pickupable>();
                    if(pickup == null)
                    {
                        Logger.Log(Logger.Level.Error, $"Couldn't find pickuppable component on instantiated object for techtype: {ingredient.techType}, failed to add resource to inventory", null, true);
                        continue;
                    }

                    Inventory.main.ForcePickup(pickup);
                }
#else
                CraftData.AddToInventory(ingredient.techType, ingredient.amount);
#endif
            }


            GameObject.Destroy(dockingBay.dockedVehicle.gameObject);
            dockingBay.dockedVehicle = null;
            dockingBay.SetVehicleUndocked();
        }

        public void OnHandHover(GUIHand hand)
        {
            if(dockingBay != null && dockingBay.dockedVehicle != null)
            {
                HandReticle.main.SetIcon(HandReticle.IconType.Hand, 1f);
#if !SN2
                HandReticle.main.SetInteractText("Deconstruct Vehicle");
#else
                HandReticle.main.SetTextRaw(HandReticle.TextType.Hand, "Deconstruct Vehicle");
#endif
            }
            else
            {
                HandReticle.main.SetIcon(HandReticle.IconType.HandDeny, 1f);
#if !SN2
                HandReticle.main.SetInteractText("No Vehicle Docked");
#else
                HandReticle.main.SetTextRaw(HandReticle.TextType.Hand, "No Vehicle Docked");
#endif
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnHandClick(null);
        }

        public void OnPointerHover(PointerEventData eventData)
        {
            OnHandHover(null);
        }
    }
}
