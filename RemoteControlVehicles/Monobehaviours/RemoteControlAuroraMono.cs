using FMOD;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UWE;
using static HangingStinger;

namespace RemoteControlVehicles.Monobehaviours
{
    public class RemoteControlAuroraMono : RemoteControlVehicle//todo: make dock. storage space maybe?
    {
        public const string ExplosionSound = "event:/creature/crash/die";
        public const TechType CrashFishPrefabType = TechType.Crash;
        public const float MaxDamage = 75;
        public const float DetonateRadius = 10;
        public const float TimeSpaceMustBeHeld = 5;

        public const float DockingBayTriggerRadius = 0.5f;
        public const float DockingBayTriggerHeight = 1;


        public static RemoteControlAuroraMono lastUsedMono { get; private set; }

        public override string VehicleName => "Aurora";

        public override Vector3 firstPersonLocalCamPos { get; set; } = new Vector3(0, 0.25f, 0.02f);
        public override Vector3 thirdPersonLocalCamPos { get; set; } = new Vector3(0, 0.45f, -0.8f);

        private float timeSpaceHeld = 0;
        private List<int> messagesPlayed = new List<int>();

        private RemoteVehicleDockingBay dockingBay;
        protected override void Start()
        {
            base.Start();
            (pickupable.droppedEvent = new Event<Pickupable>()).AddHandler(this, new Event<Pickupable>.HandleFunction(OnDropped));

            rigidBody.angularDrag = 1;

            worldForces.underwaterDrag = 2;
            worldForces.handleGravity = false;

            var triggerObj = new GameObject("Dockingbay_Trigger");
            var trigger = triggerObj.EnsureComponent<BoxCollider>();
            trigger.transform.parent = transform;
            trigger.transform.localPosition = Vector3.zero;
            trigger.transform.localRotation = Quaternion.identity;
            trigger.isTrigger = true;
            trigger.size = new Vector3(0.4f, 1.5f, 0.6f);
            trigger.center = new Vector3(0, 0, -0.25f);

            dockingBay = triggerObj.EnsureComponent<RemoteVehicleDockingBay>();

            pingInstance = EnsureComponent<PingInstance>();
            pingInstance.SetLabel("Aurora");
            pingInstance.displayPingInManager = true;
            pingInstance.origin = transform;

            //lastUsedMono is set after start is run, so if it's null the no start has run and we should register the ping type
            if (!lastUsedMono) pingInstance.pingType = SMLHelper.V2.Handlers.PingHandler.RegisterNewPingType("Aurora", SpriteManager.Get(TechType.StarshipSouvenir));
            else SMLHelper.V2.Handlers.PingHandler.TryGetModdedPingType("Aurora", out pingInstance.pingType);

            lastUsedMono = this;
        }
        protected override void Update()
        {
            base.Update();

            if (!IsControlled()) return;

            if (dockingBay.CurrentVehicle && Input.GetKeyDown(QMod.config.dockControlKey))
                dockingBay.UndockVehicle();

            if (GameInput.GetButtonHeld(GameInput.Button.LeftHand))
            {
                if (timeSpaceHeld >= 1 && !messagesPlayed.Contains(1))
                {
                    ErrorMessage.AddMessage("Beginning self destruct sequence");
                    messagesPlayed.Add(1);
                }
                else if(timeSpaceHeld >= 2 && !messagesPlayed.Contains(2))
                {
                    ErrorMessage.AddMessage("Self destruct in 3");
                    messagesPlayed.Add(2);
                }
                else if(timeSpaceHeld >= 3 && !messagesPlayed.Contains(3))
                {
                    ErrorMessage.AddMessage("Self destruct in 2");
                    messagesPlayed.Add(3);
                }
                else if (timeSpaceHeld >= 4 && !messagesPlayed.Contains(4))
                {
                    ErrorMessage.AddMessage("Self destruct in 1");
                    messagesPlayed.Add(4);
                }
                else if (timeSpaceHeld >= 5 && !messagesPlayed.Contains(5))
                {
                    SelfDestruct();
                }
                timeSpaceHeld += Time.deltaTime;
            }
            else
            {
                messagesPlayed.Clear();
                timeSpaceHeld = 0;
            }
        }
        public void OnDropped(Pickupable p)
        {
            lastUsedMono = this;
        }
        public void SelfDestruct()
        {
            var particlePrefab = CraftData.GetPrefabForTechType(CrashFishPrefabType).GetComponent<Crash>().detonateParticlePrefab;
            Utils.PlayOneShotPS(particlePrefab, transform.position, transform.rotation);
            Utils.PlayFMODAsset(GetFmodAsset(ExplosionSound), Player.main.transform);
            DamageSystem.RadiusDamage(MaxDamage, transform.position, DetonateRadius, DamageType.Explosive, gameObject);
            liveMixin.Kill();
            Destroy(gameObject);
        }
        public static FMODAsset GetFmodAsset(string audioPath)
        {
            FMODAsset asset = ScriptableObject.CreateInstance<FMODAsset>();
            asset.path = audioPath;
            return asset;
        }
    }
}
