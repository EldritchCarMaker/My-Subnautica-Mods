using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;

namespace WarpChip.Monobehaviours
{
    internal class TelePingInstance : MonoBehaviour
    {
        public static readonly List<TelePingInstance> telePings = new List<TelePingInstance>();
        public bool IsLookedAt => LookedAt();
        public bool TeleportAllowed => (_timeLastCollider + colliderExitCooldown) <= Time.time;

        private const float colliderExitCooldown = 0.5f;//time that has to pass since the last collider entered before teleport is active

        private float _timeLastCollider;
        private PingInstance _pingInstance;

        internal uGUI_Ping ping;//is set by the uGUI_Pings patch
        public void Awake()
        {
            _pingInstance = GetComponent<PingInstance>();
            _pingInstance.SetLabel("Teleping Beacon");

            var col = gameObject.EnsureComponent<SphereCollider>();
            col.isTrigger = true;
            col.radius = 3f;
        }
        public Vector3 GetSpawnPosition()
        {
            return transform.position + (0.5f * transform.forward);
        }
        public bool LookedAt()
        {
            Vector3 forward = MainCamera.camera.transform.forward;
            Vector3 position1 = MainCamera.camera.transform.position;
            Vector3 position2 = transform.position;
            Vector3 normalized = (position2 - position1).normalized;
            bool lookedAt = Vector3.Dot(forward, normalized) > 0.9848f;

            return lookedAt;
        }

        public void OnTriggerStay(Collider c)
        {
            if (c.transform.IsChildOf(Player.main.transform)) return;

            _timeLastCollider = Time.time;
        }

        public void Update()
        {
            if (TeleportAllowed) ping.SetColor(new Color(0, 1, 0));
            else ping.SetColor(new Color(1, 0, 0));
        }

        public void OnEnable()
        {
            telePings.Add(this);
        }
        public void OnDisable()
        {
            telePings.Remove(this);
        }
    }
}
