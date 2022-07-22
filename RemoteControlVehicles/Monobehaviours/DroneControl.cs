using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UWE;
using Logger = QModManager.Utility.Logger;

namespace RemoteControlVehicles.Monobehaviours
{
    public class DroneControl : PlayerTool
    {
        public static MapRoomScreen mapRoomScreenPrefab;

        public MapRoomScreen screen;
        public MapRoomFunctionality functionality;
        public StorageContainer container;

        public override void Awake()
        {
            if(TryGetComponent(out AirBladder blad)) Destroy(blad);

            base.Awake();
            pickupable = GetComponent<Pickupable>();

            screen = gameObject.EnsureComponent<MapRoomScreen>();
            screen.enabled = false;
            functionality = screen.mapRoomFunctionality = gameObject.EnsureComponent<RemoteControlMapRoomFunctionality>();

            container = GetComponentInChildren<StorageContainer>();

            if(container == null)
            {
                var root = new GameObject("RemoteStorageRoot");
                root.transform.parent = gameObject.transform;
                root.SetActive(false);

                var coi = root.EnsureComponent<ChildObjectIdentifier>();

                container = root.EnsureComponent<StorageContainer>();

                container.enabled = false;

                coi.classId = "DroneRemoteControlStorage";
                //container = coi.gameObject.EnsureComponent<StorageContainer>();
                container.prefabRoot = gameObject;
                container.storageRoot = coi;

                container.width = 2;
                container.height = 2;
                container.storageLabel = "Remote Storage";

                container.onUse = new StorageContainer.UseEvent();
                //why didn't they just add a damn null check?

                root.SetActive(true);
                container.enabled = true;
            }

            container.Resize(QMod.config.remoteStorageWidth, QMod.config.remoteStorageHeight);

            functionality.storageContainer = container;

            CoroutineHost.StartCoroutine(FinishSetup());
        }
        private IEnumerator FinishSetup()
        {
            yield return new WaitUntil(() => mapRoomScreenPrefab != null);

            screen.cameraText = mapRoomScreenPrefab.cameraText;
            screen.enabled = true;
        }
        public override bool OnRightHandDown()
        {
            if (QMod.config.MustBeInBase && !Player.main.currentSub && !Player.main.currentEscapePod)
            {
                ErrorMessage.AddMessage("Must be in safe place to control drone");
                return true;
            }

            screen.currentIndex = screen.NormalizeIndex(screen.currentIndex);

            MapRoomCamera mapRoomCamera = screen.FindCamera();
            if (mapRoomCamera)
            {
                mapRoomCamera.ControlCamera(Player.main, screen);
                screen.currentCamera = mapRoomCamera;
            }
            else
                ErrorMessage.AddMessage("Can't find drone to control");

            return base.OnRightHandDown();
        }
        public override bool OnAltDown()
        {
            container.OnHandClick(Player.main.armsController.guiHand);
            return true;
        }
        public override string GetCustomUseText()
        {
            return $"{Language.main.GetFormat<string, string>("HandReticleAddButtonFormat", "Open Storage", uGUI.FormatButton(GameInput.Button.AltTool))}";
        }
        public void Update()
        {
            if (RemoteControlVehicles.isActive)
            {
                Player.main.transform.Find("body").gameObject.SetActive(false);
            }
            else
            {
                Player.main.transform.Find("body").gameObject.SetActive(true);
            }

            if (!isDrawn || !usingPlayer) return;

            //HandReticle.main.interactText1 = $"{Language.main.GetFormat<string, string>("HandReticleAddButtonFormat", "Open Storage", uGUI.FormatButton(GameInput.Button.AltTool))}";
            //HandReticle.main.interactText2 = $"{Language.main.GetFormat<string, string>("HandReticleAddButtonFormat", "Control Drone", uGUI.FormatButton(GameInput.Button.RightHand))}";

            if (Input.GetKeyUp(QMod.config.ControlKey))
            {
                if (RemoteControlVehicles.vehicle == null)
                {
                    ErrorMessage.AddMessage("No vehicle available to control");
                    return;
                }

                if (RemoteControlVehicles.isActive)
                {
                    ErrorMessage.AddMessage("Already controlling vehicle");
                    return;
                }

                if (QMod.config.MustBeInBase && !usingPlayer.IsInSub())
                {
                    ErrorMessage.AddMessage("Must be in safe location to control vehicle");
                    return;
                }

                if (usingPlayer.currChair != null || usingPlayer.currentMountedVehicle != null)
                {
                    ErrorMessage.AddMessage("Already controlling vehicle");
                    return;
                }

                RemoteControlVehicles.sub = Player.main.currentSub;
                RemoteControlVehicles.position = Player.main.transform.position;

                if (RemoteControlVehicles.sub != null)
                {
                    RemoteControlVehicles.subPosition = RemoteControlVehicles.sub.transform.position;
                }

                RemoteControlVehicles.text1.text = RemoteControlVehicles.vehicle.subName.GetName();

                RemoteControlVehicles.hud.SetActive(true);
                RemoteControlVehicles.hud.transform.Find("Connecting").gameObject.SetActive(true);
                RemoteControlVehicles.blackScreen.SetActive(true);
                Image image = RemoteControlVehicles.blackScreen.GetComponent<Image>();
                Color color = image.color;
                color.a = 1;
                image.color = color;

                CoroutineHost.StartCoroutine(RemoteControlVehicles.WaitForWorld(usingPlayer));
            }
            if (Input.GetKeyUp(QMod.config.cyclopsControlKey))
            {
                if (CyclopsRemoteControlHandler.TrackedSub == null)
                {
                    ErrorMessage.AddMessage("No cyclops available to control");
                    return;
                }

                if (RemoteControlVehicles.isActive)
                {
                    ErrorMessage.AddMessage("Already controlling vehicle");
                    return;
                }

                if (QMod.config.MustBeInBase && !usingPlayer.IsInSub())
                {
                    ErrorMessage.AddMessage("Must be in safe location to control vehicle");
                    return;
                }

                if (usingPlayer.currChair != null || usingPlayer.currentMountedVehicle != null)
                {
                    ErrorMessage.AddMessage("Already controlling vehicle");
                    return;
                }

                RemoteControlVehicles.sub = Player.main.currentSub;
                RemoteControlVehicles.position = Player.main.transform.position;

                if (RemoteControlVehicles.sub != null)
                {
                    RemoteControlVehicles.subPosition = Player.main.currentSub.transform.position;
                }

                RemoteControlVehicles.text1.text = CyclopsRemoteControlHandler.TrackedSub.subName;

                RemoteControlVehicles.hud.SetActive(true);
                RemoteControlVehicles.hud.transform.Find("Connecting").gameObject.SetActive(true);
                RemoteControlVehicles.blackScreen.SetActive(true);
                Image image = RemoteControlVehicles.blackScreen.GetComponent<Image>();
                Color color = image.color;
                color.a = 1;
                image.color = color;


                CoroutineHost.StartCoroutine(RemoteControlVehicles.WaitForWorld(usingPlayer, true));
            }
        }
    }
}
