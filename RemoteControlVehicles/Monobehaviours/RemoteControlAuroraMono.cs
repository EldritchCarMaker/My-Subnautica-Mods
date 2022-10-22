using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UWE;

namespace RemoteControlVehicles.Monobehaviours
{
    public class RemoteControlAuroraMono : RemoteControlVehicle//todo: make explode. make dock. storage space maybe?
    {
        public static RemoteControlAuroraMono lastUsedMono { get; private set; }

        public override string VehicleName => "Aurora";

        public override Vector3 firstPersonLocalCamPos { get; set; } = new Vector3(0, 0.25f, 0.02f);
        public override Vector3 thirdPersonLocalCamPos { get; set; } = new Vector3(0, 0.45f, -0.8f);

        protected override void Start()
        {
            base.Start();
            (pickupable.droppedEvent = new Event<Pickupable>()).AddHandler(this, new Event<Pickupable>.HandleFunction(OnDropped));

            rigidBody.angularDrag = 1;

            worldForces.underwaterDrag = 2;
            worldForces.handleGravity = false;


            pingInstance = EnsureComponent<PingInstance>();
            pingInstance.SetLabel("Aurora");
            pingInstance.displayPingInManager = true;
            pingInstance.origin = transform;

            //lastUsedMono is set after start is run, so if it's null the no start has run and we should register the ping type
            if (!lastUsedMono) pingInstance.pingType = SMLHelper.V2.Handlers.PingHandler.RegisterNewPingType("Aurora", SpriteManager.Get(TechType.StarshipSouvenir));
            else SMLHelper.V2.Handlers.PingHandler.TryGetModdedPingType("Aurora", out pingInstance.pingType);

            lastUsedMono = this;
        }
        public void OnDropped(Pickupable p)
        {
            lastUsedMono = this;
        }
    }
}
