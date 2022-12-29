using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AutoStorageTransfer.Monobehaviours
{
    internal class StorageTransferControllerMono : PlayerTool
    {
        public StorageTransfer currentSeenContainer;
        public override void Awake()
        {
            base.Awake();
            pickupable = GetComponent<Pickupable>();
            Destroy(GetComponent<Welder>());
            Destroy(GetComponent<EnergyMixin>());
        }
        public override bool OnAltDown()
        {
            if (currentSeenContainer == null) return false;

            currentSeenContainer.ToggleRecieverStatus();

            return true;
        }
        public override bool OnRightHandDown()
        {
            if(currentSeenContainer == null) return false;

            uGUI.main.userInput.RequestString("ID of storage transfer", "Submit", currentSeenContainer.GetStorageID(), 20, new uGUI_UserInput.UserInputCallback(currentSeenContainer.SetIDString));
            return true;
        }
        public override bool DoesOverrideHand()
        {
            return true;
        }
        public void Update()
        {
            if (!isDrawn || usingPlayer == null)
                return;
            if (Targeting.GetTarget(Player.main.gameObject, 2, out var hitObj, out var distance))
            {
                currentSeenContainer = UWE.Utils.GetComponentInHierarchy<StorageTransfer>(hitObj);
                if(!currentSeenContainer && hitObj.transform.parent)
                {
                    currentSeenContainer = UWE.Utils.GetComponentInHierarchy<StorageTransfer>(hitObj.transform.parent.gameObject);
                }
            }
            else
                currentSeenContainer = null;

            var string1 = currentSeenContainer ? $"Set container {GetContainerName(currentSeenContainer)} ID" : "No container found";
            
            if (currentSeenContainer)
            {
#if SN
                HandReticle.main.interactText1 = $"{Language.main.GetFormat<string, string>("HandReticleAddButtonFormat", string1, uGUI.FormatButton(GameInput.Button.RightHand))}";
#else
                HandReticle.main.SetTextRaw(HandReticle.TextType.Hand, $"{Language.main.GetFormat<string, string>("HandReticleAddButtonFormat", string1, uGUI.FormatButton(GameInput.Button.RightHand))}");
#endif
                var string2 = $"Toggle reciever status. Currently {(currentSeenContainer.IsReciever ? "reciever" : "transmitter")}";
#if SN
                HandReticle.main.interactText2 = $"{Language.main.GetFormat<string, string>("HandReticleAddButtonFormat", string2, uGUI.FormatButton(GameInput.Button.AltTool))}";
#else
                HandReticle.main.SetTextRaw(HandReticle.TextType.HandSubscript, $"{Language.main.GetFormat<string, string>("HandReticleAddButtonFormat", string2, uGUI.FormatButton(GameInput.Button.AltTool))}");
#endif
            }
            else
            {
#if SN
                HandReticle.main.interactText1 = $"{string1}";
#else
                HandReticle.main.SetTextRaw(HandReticle.TextType.Hand, $"{string1}");
#endif
            }
        }
        public static string GetContainerName(MonoBehaviour obj)
        {
            if (obj == null) return string.Empty;

            var targetTechType = CraftData.GetTechType(obj.gameObject);
            var name = targetTechType == TechType.None
                ? obj.name.Replace("(Clone)", "").Replace("-MainPrefab", "")
                : Language.main.Get(targetTechType);
            return name;
        }
    }
}
