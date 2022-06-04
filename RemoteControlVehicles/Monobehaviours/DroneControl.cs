using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
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
            if(GetComponent<AirBladder>() != null) Destroy(GetComponent<AirBladder>());

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
            screen.OnHandClick(Player.main.armsController.guiHand);
            return base.OnRightHandDown();
        }
        public override bool OnAltDown()
        {
            container.OnHandClick(Player.main.armsController.guiHand);
            return true;
        }
    }
}
