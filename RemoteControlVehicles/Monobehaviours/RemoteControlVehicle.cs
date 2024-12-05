using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UWE;

namespace RemoteControlVehicles.Monobehaviours
{
    public class RemoteControlVehicle : MonoBehaviour
    {
        public float camRotationCorrectionSpeed = 5;
        public float camPositionCorrectionSpeed = 10;
        public const float cameraSmoothTime = 0.5f;

        protected virtual float acceleration { get; } = 20f;
        protected virtual float sidewaysTorque { get; } = 45f;
        protected virtual float stabilizeForce { get; } = 6f;
        protected virtual float controllTimeDelay { get; } = 0.1f;
        protected virtual float energyDrain { get; } = 0.06666f;
        protected virtual float lightEnergyDrain { get; } = 0.05f;

        public virtual string VehicleName => "Vehicle";

        public virtual Vector3 firstPersonLocalCamPos { get; set; } = new Vector3(0, 0.25f, 0.02f);
        public virtual Vector3 thirdPersonLocalCamPos { get; set; } = new Vector3(0, 0.45f, -0.8f);


        private bool isThirdPerson = false;


        protected GameObject inputStackDummy { get; private set; }
        protected Transform camTransform { get; private set; }

        public Rigidbody rigidBody { get; private set; }
        public EnergyMixin energyMixin { get; private set; }
        public LiveMixin liveMixin { get; private set; }
        public Pickupable pickupable { get; private set; }
        public WorldForces worldForces { get; private set; }

        public GameObject lightsParent { get; internal set; }
        public PingInstance pingInstance;
        public ToggleLights toggleLights { get; private set; }

        protected Vector3 wishDir = Vector3.zero;

        public Player controllingPlayer { get; private set; }
        private float controllStartTime;
        private bool readyForControl;
        private bool justStartedControl;
        private Vector3 cameraVelocity = Vector3.zero;

        public static RemoteControlVehicle currentVehicle { get; private set; }

        protected virtual void Awake()
        {
            var camObject = new GameObject("CamPositionObject");
            camTransform = camObject.transform;
            camTransform.parent = transform;
            camTransform.localPosition = firstPersonLocalCamPos;
            camTransform.localRotation = Quaternion.identity;

            inputStackDummy = new GameObject("inputStackDummy");
            inputStackDummy.transform.parent = base.transform;
            inputStackDummy.SetActive(false);

            var toggleLights = EnsureComponent<ToggleLights>();
            toggleLights.energyMixin = energyMixin;
            toggleLights.lightsParent = lightsParent;
            toggleLights.energyPerSecond = lightEnergyDrain;

            energyMixin = EnsureComponent<EnergyMixin>();
            liveMixin = EnsureComponent<LiveMixin>();
            pickupable = EnsureComponent<Pickupable>();
            rigidBody = EnsureComponent<Rigidbody>();
            worldForces = EnsureComponent<WorldForces>();
        }
        public T EnsureComponent<T>() where T : Component
        {
            return gameObject.EnsureComponent<T>();
        }
        public virtual bool IsReady()
        {
            return readyForControl && !justStartedControl;
        }

        public virtual bool CanBeControlled()
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

        public virtual void OnKill()
        {
            if (controllingPlayer)
            {
                ExitVehicle();
            }
        }

        protected virtual void OnDisable()
        {
            if (controllingPlayer)
            {
                ExitVehicle();
            }
        }

        protected virtual void OnDestroy()
        {
            if (controllingPlayer)
            {
                ExitVehicle();
            }
        }

        public void ControlVehicle()
        {
            if (!CanBeControlled()) return;

            controllStartTime = Time.time;
            controllingPlayer = Player.main;
            Player.main.EnterLockedMode(null, false);
            rigidBody.velocity = Vector3.zero;
            MainCameraControl.main.enabled = false;
            InputHandlerStack.main.Push(inputStackDummy);
            //droneIdle.Play();
            readyForControl = false;
            currentVehicle = this;
            //connectingSound.Play();
            Player.main.SetHeadVisible(true);
            lightsParent.SetActive(true);
            justStartedControl = true;
            VRUtil.Recenter();
            OnCameraControl();
        }
        protected virtual void OnCameraControl()
        {

        }

        public void ExitVehicle(bool resetPlayerPosition = true)
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
            currentVehicle = null;
            //engineSound.Stop();
            //screenEffectModel.SetActive(false);
            //droneIdle.Stop();
            //connectingSound.Stop();
            Player.main.SetHeadVisible(false);
            lightsParent.SetActive(false);
            OnCameraControl();
        }
        protected virtual void OnExitVehicle()
        {

        }

        public bool IsControlled()
        {
            return controllingPlayer != null && controllStartTime + controllTimeDelay <= Time.time;
        }

        protected virtual void UpdateEnergyRecharge()
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

        protected virtual void Update()
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
                    TurnVehicleWithCamera();
                    wishDir = GameInput.GetMoveDirection();
                    wishDir.Normalize();
                }
                else
                {
                    wishDir = Vector3.zero;
                }
                if (Input.GetKeyUp(KeyCode.Escape) || GameInput.GetButtonUp(GameInput.Button.Exit))
                {
                    ExitVehicle(true);
                }
                if (GameInput.GetButtonDown(GameInput.Button.Sprint))
                    isThirdPerson = !isThirdPerson;

                if (Input.mouseScrollDelta.y > 0) isThirdPerson = false;//go third person if scrolling back, first person if scrolling forward
                else if(Input.mouseScrollDelta.y < 0) isThirdPerson = true;

                toggleLights.CheckLightToggle();

                if (Player.main != null && Player.main.liveMixin != null && !Player.main.liveMixin.IsAlive())
                {
                    ExitVehicle(true);
                }
                HandleEngine();
            }
        }
        protected virtual void HandleEngine()
        {
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
        protected virtual void TurnVehicleWithCamera()
        {
            Vector2 lookDelta = GameInput.GetLookDelta();
            rigidBody.AddTorque(base.transform.up * lookDelta.x * sidewaysTorque * 0.0015f, ForceMode.VelocityChange);
            rigidBody.AddTorque(base.transform.right * -lookDelta.y * sidewaysTorque * 0.0015f, ForceMode.VelocityChange);
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
                SetCameraPosition();
            }
        }
        protected virtual void SetCameraPosition()
        {
            if(isThirdPerson)
            {
                if (!QMod.config.instantCamPosition)
                {
                    if(QMod.config.smoothDampCamPosition)
                        SNCameraRoot.main.transform.position = Vector3.SmoothDamp(SNCameraRoot.main.transform.position, camTransform.position, ref cameraVelocity, cameraSmoothTime);
                    else
                        SNCameraRoot.main.transform.position = Vector3.Lerp(SNCameraRoot.main.transform.position, camTransform.position, Time.deltaTime * camPositionCorrectionSpeed);
                }
                else
                {
                    SNCameraRoot.main.transform.position = camTransform.position;
                }


                if (!QMod.config.instantCamRotation)
                {
                    SNCameraRoot.main.transform.rotation = Quaternion.Lerp(SNCameraRoot.main.transform.rotation, camTransform.rotation, Time.deltaTime * camRotationCorrectionSpeed);
                }
                else
                {
                    SNCameraRoot.main.transform.rotation = camTransform.rotation;
                }
                return;
            }
            SNCameraRoot.main.transform.position = camTransform.position;
            SNCameraRoot.main.transform.rotation = camTransform.rotation;
        }

        protected virtual void StabilizeRoll()
        {
            float num = Mathf.Abs(base.transform.eulerAngles.z - 180f);
            if (num <= 178f)
            {
                float d = Mathf.Clamp(1f - num / 180f, 0f, 0.5f) * stabilizeForce;
                rigidBody.AddTorque(base.transform.forward * d * Time.deltaTime * Mathf.Sign(base.transform.eulerAngles.z - 180f), ForceMode.VelocityChange);
            }
        }

        protected virtual void FixedUpdate()
        {
            if (IsControlled())
            {
                HandleMovement();
                StabilizeRoll();
            }
        }
        protected virtual void HandleMovement()
        {
            rigidBody.AddForce(base.transform.rotation * (acceleration * wishDir), ForceMode.Acceleration);
        }
    }
}
