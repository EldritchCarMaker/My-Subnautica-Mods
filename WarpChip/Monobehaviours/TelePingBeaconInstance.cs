using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WarpChip.Monobehaviours
{
    public class TelePingBeaconInstance : TelePingInstance
    {
        private const float colliderExitCooldown = 0.5f;//time that has to pass since the last collider entered before teleport is active
        private float _timeLastCollider;
        public override bool TeleportAllowed => (_timeLastCollider + colliderExitCooldown) <= Time.time;
        internal bool precursorOutOfWater;//set by beacon throw patch

        public override void Awake()
        {
            base.Awake();

            _pingInstance.SetLabel("Teleping Beacon");

            var col = gameObject.EnsureComponent<SphereCollider>();
            col.isTrigger = true;
            col.radius = 3f;
        }

        public override void Teleport()
        {
            Player.main.OnPlayerPositionCheat();
            Player.main.SetPosition(GetSpawnPosition());
            Player.main.SetPrecursorOutOfWater(precursorOutOfWater);
        }

        public void OnTriggerStay(Collider c)
        {
            if (c.isTrigger) return;
            if (c.transform.IsChildOf(Player.main.transform)) return;

            _timeLastCollider = Time.time;
        }

        private Vector3 GetSpawnPosition()
        {
            return transform.position + (0.5f * transform.forward);
        }
    }
}
