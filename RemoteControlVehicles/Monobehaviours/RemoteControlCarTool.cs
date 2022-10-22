using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UWE;

namespace RemoteControlVehicles.Monobehaviours
{
    internal class RemoteControlCarTool : PlayerTool
    {
        private const float dropForceAmount = 20f;
        public WorldForces worldForces;

        public override void Awake()
        {
            base.Awake();

            Destroy(GetComponent<PlaceTool>());
            Destroy(GetComponent<FPModel>());

            socket = Socket.RightHand;
            drawTime = 0.5f;
            holsterTime = 0.35f;
            dropTime = 0.3f;
            reloadMode = ReloadMode.Direct;
            mainCollider = GetComponentInChildren<Collider>();

            worldForces = GetComponent<WorldForces>();
            pickupable = GetComponent<Pickupable>();
            if (!pickupable.attached)
            {
                OnDropped(pickupable);
            }
            worldForces.handleGravity = false;

            pickupable.droppedEvent.AddHandler(gameObject, new Event<Pickupable>.HandleFunction(OnDropped));
        }
        public override string animToolName => "Remote Control Car";

        public override bool OnRightHandDown()
        {
            _isInUse = false;
            pickupable.Drop(Player.main.camRoot.GetAimingTransform().position + Player.main.camRoot.GetAimingTransform().forward * 2);
            transform.rotation = Quaternion.LookRotation(Player.main.transform.position);
            return true;
        }

        private void OnDropped(Pickupable pickupable)
        {
            Rigidbody component = GetComponent<Rigidbody>();
            component.AddForce(Player.main.camRoot.GetAimingTransform().forward * dropForceAmount);
        }
    }
}
