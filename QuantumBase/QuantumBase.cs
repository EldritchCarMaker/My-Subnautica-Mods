using QuantumBase.Mono;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UWE;
using Valve.VR;
using static OVRPlugin;

namespace QuantumBase
{
    public class QuantumBase : SubRoot
    {
        public static QuantumBase main { get; private set; }


        private GameObject respawnObject;
        private bool firstTeleport = true;
        public static void EnsureBase()
        {
            if (main) return;
            Instantiate(new QuantumBasePrefab().GetGameObject());
        }

        public override void Awake()
        {
            if(main)
            {
                QModManager.Utility.Logger.Log(QModManager.Utility.Logger.Level.Warn, "Two quantum bases found!");
                Destroy(gameObject);
                return;
            }

            main = this;

            isCyclops = false;
            isBase = true;


            Vector3 basePosition = new Vector3(940.5f, 60.4f, -285.8f);


            transform.position = basePosition;

            LOD = EnsureComponent<BehaviourLOD>();
            rigidbody = GetComponent<Rigidbody>();
            modulesRoot = transform;

            respawnObject = new GameObject("RespawnPoint");
            respawnObject.EnsureComponent<RespawnPoint>();
            respawnObject.transform.parent = transform;


            Vector3 localSpawnEuler = new Vector3(90, 0, 0);
            Vector3 localSpawnPosition = new Vector3(0.5f, 3.6f, -3.3f);


            respawnObject.transform.localEulerAngles = localSpawnEuler;
            respawnObject.transform.localPosition = localSpawnPosition;

            dimFloodlightsOnEnter = new VFXVolumetricLight[0];

            GetComponent<SkyApplier>().SetSky(Skies.BaseInterior);

            SetUpPowerRelay();
        }
        private void SetUpPowerRelay()
        {
            powerRelay = EnsureComponent<PowerRelay>();

            powerRelay.internalPowerSource = EnsureComponent<PowerSource>();

            powerRelay.internalPowerSource.connectedRelay = powerRelay;

            var regen = EnsureComponent<RegeneratePowerSource>();
            regen.powerSource = powerRelay.internalPowerSource;
            regen.regenerationInterval = 1;
            regen.regenerationAmount = 2;
        }

        private IEnumerator SetUpDoors()
        {
            const string DOOR_CLASSID = "b86d345e-0517-4f6e-bea4-2c5b40f623b4";

            var doorRequest = PrefabDatabase.GetPrefabAsync(DOOR_CLASSID);
            yield return doorRequest;
            doorRequest.TryGetPrefab(out var prefab);

            var door = Instantiate(prefab);
            door.transform.parent = transform;

            Destroy(door.GetComponent<LargeWorldEntity>());

            Vector3 localDoorPosition = new Vector3(0.47f, -0.19f, -6.67f);


            door.transform.localPosition = localDoorPosition;
            door.transform.eulerAngles = Vector3.zero;

            var SSDoor = door.GetComponent<StarshipDoor>();
            SSDoor.doorLocked = false;
            SSDoor.openText = "Exit Base";
            SSDoor.gameObject.EnsureComponent<QuantumExitDoor>();
        }
        public override void Start()
        {
            LoadSaveData();

            QMod.SaveData.OnStartedSaving += OnSave;

            CoroutineHost.StartCoroutine(SetUpDoors());

            CoroutineHost.StartCoroutine(SetUpWalls());

            CoroutineHost.StartCoroutine(SetUpBioReactor());
        }
        private void OnSave(object sender, EventArgs e)
        {
            QMod.SaveData.Energy = powerRelay.internalPowerSource.GetPower();

            QMod.SaveData.isInBase = Player.main.currentSub is QuantumBase;

            QMod.SaveData.LastPosition = lastPosition;
            QMod.SaveData.wasInLifepod = wasInLifepod;

            QMod.SaveData.LastSubRoot = lastSub ? lastSub.GetComponent<UniqueIdentifier>().id : null;
            QMod.SaveData.LastVehicle = lastVehicle ? lastVehicle.GetComponent<UniqueIdentifier>().id : null;
        }
        private void LoadSaveData()
        {
            powerRelay.internalPowerSource.SetPower(QMod.SaveData.Energy);

            if (QMod.SaveData.isInBase) Player.main.currentSub = this;

            lastPosition = QMod.SaveData.LastPosition;
            wasInLifepod = QMod.SaveData.wasInLifepod;
        }
        private IEnumerator SetUpBioReactor()
        {
            yield return new WaitUntil(() => Base.pieces != null && Base.pieces.Length > 0 && Base.pieces[(int)Base.Piece.RoomBioReactor].prefab != null);

            var reactorPrefab = Base.pieces[(int)Base.Piece.RoomBioReactor].prefab.gameObject;

            var reactor = Instantiate(reactorPrefab);
            Destroy(GetComponent<BaseBioReactorGeometry>());
            reactor.SetActive(true);


            var reactorPosition = new Vector3(0.57f, 4.59f, -23.5f);


            reactor.transform.SetParent(transform);
            reactor.transform.localPosition = reactorPosition;


            var handTarget = reactor.GetComponentInChildren<GenericHandTarget>();
            var energyGen = handTarget.gameObject.EnsureComponent<EnergyGenerator>();
            energyGen.powerSource = powerRelay.internalPowerSource;
            Destroy(handTarget);
        }
        private IEnumerator SetUpWalls()
        {
            var task = CraftData.GetPrefabForTechTypeAsync(TechType.SingleWallShelf, false);
            yield return task;
            var prefab = task.GetResult();

            Dictionary<Vector3, Vector3> WallSizesAndPositions = new Dictionary<Vector3, Vector3>()
            {
                { new Vector3(13, 2, 10), new Vector3(-7.06f, 18.97f, -26.5f) },
                { new Vector3(11, 2, 10), new Vector3(-17f, 9.65f, -26.5f) },
                { new Vector3(7, 2, 10), new Vector3(2.8f, 4, -26.5f) },
            };

            bool firstOne = true;//first one has different rotation than the rest
            foreach(var SizeAndPosition in WallSizesAndPositions)
            {
                var wall = Instantiate(prefab);
                Destroy(wall.GetComponent<Constructable>());
                wall.transform.parent = transform;
                wall.transform.localScale = SizeAndPosition.Key;
                wall.transform.localPosition = SizeAndPosition.Value;
                wall.transform.localEulerAngles = new Vector3(0, 359f, firstOne ? 0 : 270);

                firstOne = false;
            }

            var floor = Instantiate(prefab);
            Destroy(floor.GetComponent<Constructable>());
            floor.transform.parent = transform;
            floor.transform.localScale = new Vector3(17, 2, 43);
            floor.transform.localPosition = new Vector3(44.68f, 59, -25.84f);
            floor.transform.localEulerAngles = new Vector3(0, 269, 90);

            yield return SetUpWindow(prefab);
        }
        private IEnumerator SetUpWindow(GameObject shelfPrefab)
        {
            const string KEYPAD_CLASSID = "c8e53aa9-f599-4194-b2b1-c1645d001a82";

            var request = PrefabDatabase.GetPrefabAsync(KEYPAD_CLASSID);
            yield return request;
            request.TryGetPrefab(out var prefab);

            var keypadPrefab = prefab.FindChild("Keypad1");

            var keypad = Instantiate(keypadPrefab);
            keypad.transform.parent = transform;


            Vector3 keypadPosition = new Vector3(3.30f, 6, -26.37f);


            keypad.transform.localPosition = keypadPosition;
            keypad.transform.localEulerAngles = new Vector3(0, 270, 270);

            var windowObject = new GameObject("WindowParent");
            windowObject.transform.parent = transform;
            windowObject.transform.localPosition = Vector3.zero;
            var keypadComp = keypad.GetComponent<KeypadDoorConsole>();
            keypadComp.root = windowObject;
            keypadComp.accessCode = "5492";
            var win = windowObject.EnsureComponent<WindowMono>();
            win.MakeWindow(shelfPrefab);
            win.keypad = keypadComp;
        }
        public void SetPlayerInBase()
        {
            ErrorMessage.AddMessage("Teleporting player to base");

            lastPosition = Player.main.transform.position;
            wasInLifepod = Player.main.currentEscapePod;//wow, this looks so cursed somehow? no explicit bool cast necessary here
            lastSub = Player.main.currentSub;
            lastVehicle = Player.main.GetVehicle();
            if (lastVehicle)
            {
                lastVehicle.OnPilotModeEnd();
                if (!Player.main.ToNormalMode(true))
                {
                    Player.main.ToNormalMode(false);
                    Player.main.transform.parent = null;
                }
                //do shit here
            }

            Player.main.SetPosition(respawnObject.transform.position, respawnObject.transform.rotation);
            Player.main.currentSub = this;
            Player.main.transform.localScale = Vector3.one;

            CoroutineHost.StartCoroutine(TeleportFX());
        }
        public void SetPlayerOutBase()
        {
            ErrorMessage.AddMessage("Teleporting Player Back");

            Player.main.SetPosition(lastPosition);

            if (wasInLifepod) Player.main.currentEscapePod = EscapePod.main;


            //check for vehicles from save data on the first teleport, otherwise trust that the fields are correct
            if (firstTeleport && !lastSub && !string.IsNullOrEmpty(QMod.SaveData.LastSubRoot))
                lastSub = FindObjectsOfType<SubRoot>().FirstOrDefault((sub) => { return (bool)(sub.GetComponent<UniqueIdentifier>()?.Id.Equals(QMod.SaveData.LastSubRoot)); });

            if (firstTeleport && !lastVehicle && !string.IsNullOrEmpty(QMod.SaveData.LastVehicle))
                lastVehicle = FindObjectsOfType<Vehicle>().FirstOrDefault((vehicle) => { return (bool)(vehicle.GetComponent<UniqueIdentifier>()?.Id.Equals(QMod.SaveData.LastVehicle)); });

            firstTeleport = false;


            Player.main.currentSub = lastSub;
            if (lastVehicle && !lastVehicle.docked)
            {
                lastVehicle.EnterVehicle(Player.main, true, false);
            }
            CoroutineHost.StartCoroutine(TeleportFX());
        }
        public static IEnumerator TeleportFX(float delay = 1f)
        {
            TeleportScreenFXController fxController = MainCamera.camera.GetComponent<TeleportScreenFXController>();
            fxController.StartTeleport();
            yield return new WaitForSeconds(delay);
            fxController.StopTeleport();
        }
        private T EnsureComponent<T>() where T : MonoBehaviour
        {
            return gameObject.EnsureComponent<T>();
        }


        private bool wasInLifepod;
        private Vector3 lastPosition;
        private SubRoot lastSub;
        private Vehicle lastVehicle;
    }
}
