using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using UWE;

namespace WarpChip.Monobehaviours
{
    internal class TelePingInstance : MonoBehaviour
    {
        private static readonly List<TelePingInstance> telePings = new List<TelePingInstance>();
        public bool IsLookedAt => LookedAt();
        public bool TeleportAllowed => (_timeLastCollider + colliderExitCooldown) <= Time.time;

        private const float colliderExitCooldown = 0.5f;//time that has to pass since the last collider entered before teleport is active
        private const float lookAngleDifferenceMax = 0.00025f;//difference between two beacons look angle before they are considered "overlapping" and sorted by distance

        private float _timeLastCollider;
        private PingInstance _pingInstance;

        internal uGUI_Ping ping;//is set by the uGUI_Pings patch
        internal bool precursorOutOfWater;//set by beacon throw patch
        public void Awake()
        {
            _pingInstance = gameObject.EnsureComponent<PingInstance>();
            _pingInstance.SetLabel("Teleping Beacon");

            var col = gameObject.EnsureComponent<SphereCollider>();
            col.isTrigger = true;
            col.radius = 3f;
        }
        public void Teleport()
        {
            Player.main.OnPlayerPositionCheat();
            Player.main.SetPosition(GetSpawnPosition());
            Player.main.SetPrecursorOutOfWater(precursorOutOfWater);
        }
        private Vector3 GetSpawnPosition()
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
        public float GetLookAngleDifference()
        {
            Vector3 forward = MainCamera.camera.transform.forward;
            Vector3 position1 = MainCamera.camera.transform.position;
            Vector3 position2 = transform.position;
            Vector3 normalized = (position2 - position1).normalized;
            return Vector3.Dot(forward, normalized);
        }

        public void OnTriggerStay(Collider c)
        {
            if (c.isTrigger) return;
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

        public static TelePingInstance GetTelePing()
        {
            TelePingInstance result = null;

            float highest = 0.9848f;//default game's value for angle difference where beacon is considered "looked at"
            float lowestDistance = float.MaxValue;

            foreach (var telePing in telePings)
            {
                if (!telePing.TeleportAllowed) continue;

                var lookAngle = telePing.GetLookAngleDifference();
                var distance = Vector3.SqrMagnitude(telePing.transform.position - MainCamera.camera.transform.position);

                //whether the two beacons are considered overlapping and should sort by distance, rather than sorting by which one is in target more
                bool sortByDistance = Mathf.Abs(lookAngle - highest) < lookAngleDifferenceMax;

                if (lookAngle > highest)
                {
                    highest = lookAngle;
                    if(!sortByDistance)
                        result = telePing;
                }

                if(distance < lowestDistance)
                {
                    lowestDistance = distance;
                    if (sortByDistance)
                        result = telePing;
                }
            }
            return result;
        }
    }
}
