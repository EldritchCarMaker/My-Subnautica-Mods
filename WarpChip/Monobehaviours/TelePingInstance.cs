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
    public abstract class TelePingInstance : MonoBehaviour
    {
        private static readonly List<TelePingInstance> telePings = new List<TelePingInstance>();
        public bool IsLookedAt => LookedAt();
        public abstract bool TeleportAllowed { get; }
        public virtual bool ShouldUseCinematicMode => true;//normal beacons need it, vehicles break with it.

        private const float lookAngleDifferenceMax = 0.00025f;//difference between two beacons look angle before they are considered "overlapping" and sorted by distance

        protected PingInstance _pingInstance;

        internal uGUI_Ping ping;//is set by the uGUI_Pings patch
        public virtual void Awake()
        {
            _pingInstance = gameObject.EnsureComponent<PingInstance>();
        }
        public abstract void Teleport();
        private bool LookedAt()
        {
            Vector3 forward = MainCamera.camera.transform.forward;
            Vector3 position1 = MainCamera.camera.transform.position;
            Vector3 position2 = transform.position;
            Vector3 normalized = (position2 - position1).normalized;
            bool lookedAt = Vector3.Dot(forward, normalized) > 0.9848f;

            return lookedAt;
        }
        private float GetLookAngleDifference()
        {
            Vector3 forward = MainCamera.camera.transform.forward;
            Vector3 position1 = MainCamera.camera.transform.position;
            Vector3 position2 = transform.position;
            Vector3 normalized = (position2 - position1).normalized;
            return Vector3.Dot(forward, normalized);
        }

        public virtual void Update()
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
