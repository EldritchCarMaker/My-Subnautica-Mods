using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LuckyBlock.MonoBehaviours
{
    internal class BlockMover : MonoBehaviour
    {
        public static float torque = 0.001f;
        public static float force = 0.01f;
        public Rigidbody body;
        public void FixedUpdate()
        {
            body.AddTorque(0, torque, 0, ForceMode.VelocityChange);
            body.AddForce(force, 0, 0, ForceMode.VelocityChange);
        }
    }
}
