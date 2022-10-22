using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UWE;

namespace RemoteControlVehicles.Monobehaviours
{
    internal class RemoteControlCarMono : MonoBehaviour
    {
        public static RemoteControlCarMono lastUsedMono { get; private set; }

        private const float acceleration = 20f;
        private const float stabilizeForce = 10f;
        private const float controllTimeDelay = 0.1f;
        private const float energyDrain = 0.06666f;
        private const float lightEnergyDrain = 0.5f;
        private static float MaxTurnRadius = 45;
        private static float TurnRadiusPerSecond = 20;


        public static Vector3 firstPersonLocalCamPos = new Vector3(0, 0.19f, -0.02f);
        public static Vector3 thirdPersonLocalCamPos = new Vector3(0, 0.55f, -1f);
        //public static Vector3 forceApplyObjectLocalPos = new Vector3(0, 0.19f, -0.34f);
        public static Vector3 forceApplyObjectLocalPos = new Vector3(0, 0.1f, -0.25f);


        private bool isThirdPerson = false;


        private GameObject inputStackDummy;
        private Transform camTransform;
        private Transform forceApplyTransform;

        public Rigidbody rigidBody;
        public EnergyMixin energyMixin;
        public LiveMixin liveMixin;
        public Pickupable pickupable;
        public WorldForces worldForces;

        public GameObject lightsParent;
        public PingInstance pingInstance;
        public ToggleLights toggleLights;

        public Player controllingPlayer { get; private set; }
        private float controllStartTime;
        private bool readyForControl;
        private bool justStartedControl;

        private void Start()
        {
            var camObject = new GameObject("CamPositionObject");
            camTransform = camObject.transform;
            camTransform.parent = transform;
            camTransform.localPosition = firstPersonLocalCamPos;
            camTransform.localRotation = Quaternion.identity;

            var forceObject = new GameObject("ForceApplyObject");
            forceApplyTransform = forceObject.transform;
            forceApplyTransform.parent = transform;
            forceApplyTransform.localPosition = forceApplyObjectLocalPos;
            forceApplyTransform.localRotation = Quaternion.identity;

            inputStackDummy = new GameObject("inputStackDummy");
            inputStackDummy.transform.parent = base.transform;
            inputStackDummy.SetActive(false);

            var camPrefab = CraftData.GetPrefabForTechType(TechType.MapRoomCamera).GetComponent<MapRoomCamera>();

            lightsParent = Instantiate(camPrefab.lightsParent);
            lightsParent.SetActive(false);
            lightsParent.transform.parent = transform;
            lightsParent.transform.localRotation = Quaternion.identity;
            lightsParent.transform.localPosition = Vector3.zero;


            energyMixin = EnsureComponent<EnergyMixin>();
            energyMixin.compatibleBatteries = camPrefab.energyMixin.compatibleBatteries;
            energyMixin.storageRoot = (new GameObject("BatteryRoot").AddComponent<ChildObjectIdentifier>());
            energyMixin.Initialize();//a mod is fucking with the Awake method in a prefix, have to initialize manually
            energyMixin.batteryModels = new EnergyMixin.BatteryModels[0];
            energyMixin.controlledObjects = new GameObject[0];


            toggleLights = EnsureComponent<ToggleLights>();
            toggleLights.lightsParent = lightsParent;
            toggleLights.energyMixin = energyMixin;
            toggleLights.energyPerSecond = lightEnergyDrain;


            EnsureComponent<LargeWorldEntity>().cellLevel = LargeWorldEntity.CellLevel.Global;


            liveMixin = EnsureComponent<LiveMixin>();
            liveMixin.data = ScriptableObject.CreateInstance<LiveMixinData>();
            liveMixin.data.maxHealth = 100;
            liveMixin.data.destroyOnDeath = false;
            liveMixin.data.weldable = true;
            liveMixin.initialHealth = 100;
            liveMixin.ResetHealth();


            pickupable = EnsureComponent<Pickupable>();
            (pickupable.droppedEvent = new Event<Pickupable>()).AddHandler(this, new Event<Pickupable>.HandleFunction(OnDropped));


            rigidBody = EnsureComponent<Rigidbody>();
            rigidBody.angularDrag = 1;


            worldForces = EnsureComponent<WorldForces>();
            worldForces.handleGravity = true;
            worldForces.underwaterDrag = 2;
            worldForces.underwaterGravity = 9.81f;

            pingInstance = EnsureComponent<PingInstance>();
            pingInstance.SetLabel("RC Car");
            pingInstance.displayPingInManager = true;
            pingInstance.origin = transform;

            //lastUsedMono is set after start is run, so if it's null the no start has run and we should register the ping type
            if(!lastUsedMono) pingInstance.pingType = SMLHelper.V2.Handlers.PingHandler.RegisterNewPingType("RCCar", SpriteManager.Get(TechType.ToyCar));
            else SMLHelper.V2.Handlers.PingHandler.TryGetModdedPingType("RCCar", out pingInstance.pingType);

            lastUsedMono = this;
        }
        public T EnsureComponent<T>() where T : Component
        {
            return gameObject.EnsureComponent<T>();
        }
        public void OnDropped(Pickupable p)
        {
            lastUsedMono = this;
        }
        public bool IsReady()
        {
            return readyForControl && !justStartedControl;
        }

        public bool CanBeControlled()
        {
            return (energyMixin.charge > 0f || !GameModeUtils.RequiresPower()) && liveMixin.IsAlive() && !pickupable.attached && isActiveAndEnabled;
        }

        public float GetDistance()
        {
            return (transform.position - Player.main.transform.position).magnitude;
        }

        public float GetDepth()
        {
            return Mathf.Abs(Mathf.Min(0f, transform.position.y));
        }

        public void OnKill()
        {
            if (controllingPlayer)
            {
                FreeCamera(true);
            }
        }

        private void OnDisable()
        {
            if (controllingPlayer)
            {
                FreeCamera(true);
            }
        }

        private void OnDestroy()
        {
            if (controllingPlayer)
            {
                FreeCamera(true);
            }
        }

        public void ControlCar()
        {
            controllStartTime = Time.time;
            controllingPlayer = Player.main;
            Player.main.EnterLockedMode(null, false);
            rigidBody.velocity = Vector3.zero;
            MainCameraControl.main.enabled = false;
            InputHandlerStack.main.Push(inputStackDummy);
            //set remote control screen shit here
            //screenEffectModel.SetActive(true);
            //droneIdle.Play();
            readyForControl = false;
            //connectingSound.Play();
            Player.main.SetHeadVisible(true);
            lightsParent.SetActive(true);
            justStartedControl = true;
            VRUtil.Recenter();
        }

        public void FreeCamera(bool resetPlayerPosition = true)
        {
            InputHandlerStack.main.Pop(inputStackDummy);
            controllingPlayer.ExitLockedMode(false, false);
            controllingPlayer = null;
            if (resetPlayerPosition)
            {
                SNCameraRoot.main.transform.localPosition = Vector3.zero;
                SNCameraRoot.main.transform.localRotation = Quaternion.identity;
            }
            rigidBody.velocity = Vector3.zero;
            MainCameraControl.main.enabled = true;
            //RenderToTexture();
            uGUI_CameraDrone.main.SetCamera(null);
            uGUI_CameraDrone.main.SetScreen(null);
            //engineSound.Stop();
            //screenEffectModel.SetActive(false);
            //droneIdle.Stop();
            //connectingSound.Stop();
            Player.main.SetHeadVisible(false);
            lightsParent.SetActive(false);
        }

        private bool IsControlled()
        {
            return controllingPlayer != null && controllStartTime + controllTimeDelay <= Time.time;
        }

        private void UpdateEnergyRecharge()
        {
            bool flag = false;
            float charge = energyMixin.charge;
            float capacity = energyMixin.capacity;
            if (/*dockingPoint != null && */charge < capacity)
            {
                float amount = Mathf.Min(capacity - charge, capacity * 0.1f);
                /*
                PowerRelay componentInParent = dockingPoint.GetComponentInParent<PowerRelay>();
                if (componentInParent == null)
                {
                    Debug.LogError("camera drone is docked but can't access PowerRelay component");
                }
                float num = 0f;
                componentInParent.ConsumeEnergy(amount, out num);
                if (!GameModeUtils.RequiresPower() || num > 0f)
                {
                    energyMixin.AddEnergy(num);
                    flag = true;
                }*/
            }
            if (flag)
            {
                //chargingSound.Play();
                return;
            }
            //chargingSound.Stop();
        }

        private void Update()
        {
            UpdateEnergyRecharge();
            if (IsControlled() && inputStackDummy.activeInHierarchy)
            {
                if (!IsReady() && LargeWorldStreamer.main.IsWorldSettled())
                {
                    readyForControl = true;
                    //connectingSound.Stop();
                    //Utils.PlayFMODAsset(connectedSound, base.transform, 20f);
                }
                if (CanBeControlled() && readyForControl)
                {
                    /*
                    Vector2 lookDelta = GameInput.GetLookDelta();
                    rigidBody.AddTorque(base.transform.up * lookDelta.x * sidewaysTorque * 0.0015f, ForceMode.VelocityChange);
                    rigidBody.AddTorque(base.transform.right * -lookDelta.y * sidewaysTorque * 0.0015f, ForceMode.VelocityChange);
                    wishDir = GameInput.GetMoveDirection();
                    wishDir.Normalize();
                    */
                }
                else
                {
                    //wishDir = Vector3.zero;
                }
                if (Input.GetKeyUp(KeyCode.Escape) || GameInput.GetButtonUp(GameInput.Button.Exit))
                {
                    FreeCamera(true);
                }
                if (GameInput.GetButtonDown(GameInput.Button.Sprint))
                    isThirdPerson = !isThirdPerson;

                toggleLights.CheckLightToggle();

                if (Player.main != null && Player.main.liveMixin != null && !Player.main.liveMixin.IsAlive())
                {
                    FreeCamera(true);
                }
                float magnitude = rigidBody.velocity.magnitude;
                //float time = Mathf.Clamp(base.transform.InverseTransformDirection(base.GetComponent<Rigidbody>().velocity).z / 15f, 0f, 1f);
                if (magnitude > 2f)
                {
                    //engineSound.Play();
                    energyMixin.ConsumeEnergy(Time.deltaTime * energyDrain);
                }
                else
                {
                    //engineSound.Stop();
                }
            }
        }

        private void LateUpdate()
        {
            if (controllingPlayer)
            {
                if (justStartedControl)
                {
                    justStartedControl = false;
                    return;
                }
                camTransform.localPosition = isThirdPerson ? thirdPersonLocalCamPos : firstPersonLocalCamPos;

                SNCameraRoot.main.transform.position = camTransform.position;
                SNCameraRoot.main.transform.rotation = camTransform.rotation;
            }
        }

        private void StabilizeRoll()
        {
            float num = Mathf.Abs(base.transform.eulerAngles.z - 180f);
            if (num <= 178f)
            {
                float d = Mathf.Clamp(1f - num / 180f, 0f, 0.5f) * stabilizeForce;
                rigidBody.AddTorque(base.transform.forward * d * Time.deltaTime * Mathf.Sign(base.transform.eulerAngles.z - 180f), ForceMode.VelocityChange);
            }
        }

        private void FixedUpdate()
        {
            if (IsControlled())
            {
                var desiredDirection = GameInput.GetMoveDirection().normalized;

                Quaternion targetRotation = Quaternion.identity;
                if (desiredDirection.x < 0) targetRotation = Quaternion.Euler(0, -MaxTurnRadius, 0);
                else if (desiredDirection.x > 0) targetRotation = Quaternion.Euler(0, MaxTurnRadius, 0);
                forceApplyTransform.localRotation = Quaternion.RotateTowards(transform.localRotation, targetRotation, Time.deltaTime * TurnRadiusPerSecond);

                rigidBody.AddForceAtPosition(forceApplyTransform.forward * (acceleration * desiredDirection.z), forceApplyTransform.position, ForceMode.Acceleration);
                StabilizeRoll();
            }
        }
    }
}
