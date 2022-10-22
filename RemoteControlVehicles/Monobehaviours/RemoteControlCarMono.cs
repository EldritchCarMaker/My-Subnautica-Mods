using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UWE;

namespace RemoteControlVehicles.Monobehaviours
{
    internal class RemoteControlCarMono : RemoteControlVehicle//todo: allow camera to look around
    {
        public static RemoteControlCarMono lastUsedMono { get; private set; }

        public override string VehicleName => "RC Car";

        private static float maxSpeed = 40f;//not actually max speed, just the value for which it is considered max speed for turning. less than this, and turning is less effective

        //private const float MaxTurnRadius = 45;
        private static float turnRadiusPerSecond = 10;
        private const float maxStabilizeForce = 50;
        private const float minStabilizeForce = 15;
        private const float maxTimeSpaceHeld = 2;
        private const float jumpCooldown = 7;
        private const float jumpForce = 20;


        public override Vector3 firstPersonLocalCamPos => new Vector3(0, 0.19f, -0.02f);
        public override Vector3 thirdPersonLocalCamPos => new Vector3(0, 0.55f, -1f);


        //makes it so that stabilize force increases as the car gets more and more flipped
        protected override float stabilizeForce 
        { 
            get 
            {
                var tiltedPercent = (-Vector3.Dot(transform.up, Vector3.up) / 2) + 0.5f;

                return Mathf.Lerp(minStabilizeForce, maxStabilizeForce, tiltedPercent); 
            } 
        }

        private bool isHovering = false;
        private float timeSpaceHeld = 0;//how long space has been held for
        private float timeNextJump;
        private bool justSwappedHovering = false;

        private DockableRemoteVehicle dockable;

        protected override void Start()
        {
            base.Start();

            EnsureComponent<FreezeRigidbodyWhenFar>().freezeDist = 50;

            (pickupable.droppedEvent = new Event<Pickupable>()).AddHandler(this, new Event<Pickupable>.HandleFunction(OnDropped));

            rigidBody.angularDrag = 1;
            rigidBody.mass = 2000;

            worldForces.handleGravity = true;
            worldForces.underwaterDrag = 2;
            worldForces.aboveWaterDrag = 2;
            worldForces.underwaterGravity = 9.81f;

            dockable = EnsureComponent<DockableRemoteVehicle>();

            pingInstance = EnsureComponent<PingInstance>();
            pingInstance.SetLabel("RC Car");
            pingInstance.displayPingInManager = true;
            pingInstance.origin = transform;

            //lastUsedMono is set after start is run, so if it's null the no start has run and we should register the ping type
            if(!lastUsedMono) pingInstance.pingType = SMLHelper.V2.Handlers.PingHandler.RegisterNewPingType("RCCar", SpriteManager.Get(TechType.ToyCar));
            else SMLHelper.V2.Handlers.PingHandler.TryGetModdedPingType("RCCar", out pingInstance.pingType);

            lastUsedMono = this;
        }
        public void OnDropped(Pickupable p)
        {
            lastUsedMono = this;
        }
        
        protected override void Update()
        {
            base.Update();

            if (!IsControlled()) return;

            if(!isHovering && Time.time >= timeNextJump && GameInput.GetButtonUp(GameInput.Button.Jump))
            {
                if(justSwappedHovering)
                {
                    justSwappedHovering = false;
                    return;
                }

                timeNextJump = Time.time + jumpCooldown;
                rigidBody.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
            }
            else if (GameInput.GetButtonHeld(GameInput.Button.Jump))
            {
                timeSpaceHeld += Time.deltaTime;
                if (timeSpaceHeld > maxTimeSpaceHeld)
                {
                    timeSpaceHeld = 0;
                    justSwappedHovering = true;
                    isHovering = !isHovering;
                    ErrorMessage.AddMessage(isHovering ? "Now hovering above water" : "Stopped hovering");
                }
            }
            else
            {
                timeSpaceHeld = 0;
            }
            if(Input.GetKeyUp(KeyCode.LeftControl))
            {
                transform.rotation = Quaternion.identity;
                rigidBody.velocity = Vector3.zero;
            }
            UpdateGravity();
        }
        private void UpdateGravity()
        {
            
            if (isHovering)
            {
                worldForces.underwaterGravity = -7;
                worldForces.aboveWaterGravity = -7;
                worldForces.handleGravity = transform.position.y < 1 ? true : false;//don't do gravity when above water
                transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, transform.eulerAngles.z);
            }
            else
            {
                worldForces.underwaterGravity = 7;
                worldForces.aboveWaterGravity = 7;
                worldForces.handleGravity = true;
            }
        }
        protected override void TurnVehicleWithCamera()
        {
            //don't want to move camera here
        }
        protected override void StabilizeRoll()
        {
            base.StabilizeRoll();
            float num = Mathf.Abs(base.transform.eulerAngles.x - 180f);
            if (num <= 178f)
            {
                float d = Mathf.Clamp(1f - num / 180f, 0f, 0.5f) * stabilizeForce;
                rigidBody.AddTorque(transform.forward * d * Time.deltaTime * Mathf.Sign(base.transform.eulerAngles.x - 180f), ForceMode.VelocityChange);
            }
        }
        protected override void HandleMovement()
        {
            var rotationSpeed = wishDir.x * Mathf.Sign(wishDir.z) * turnRadiusPerSecond * (Mathf.Clamp01(rigidBody.velocity.magnitude / maxSpeed));
            transform.eulerAngles += new Vector3(0, rotationSpeed, 0);

            rigidBody.AddForce(transform.forward * (acceleration * wishDir.z), ForceMode.Acceleration);
        }
    }
}
