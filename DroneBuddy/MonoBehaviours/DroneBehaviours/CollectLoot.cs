using DroneBuddy.Patches;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DroneBuddy.MonoBehaviours.DroneBehaviours;

internal class CollectLoot : DroneBehaviour
{
    private CollectableLoot currentLootTarget;
    public override Drone.Mode BehaviourMode => Drone.Mode.Far;
    private static float maxCollectDist = 30;
    private static float maxGrabLootDist = 2;
    public override bool UpdateTargetPosition(out Vector3 target)
    {
        if(currentLootTarget == null)
        {
            //ErrorMessage.AddMessage("Looking for node");
            currentLootTarget = FindTarget();
            //ErrorMessage.AddMessage(currentLootTarget != null ? $"Found node at {currentLootTarget.Position}" : "Couldn't find node");
        }

        if (currentLootTarget == null)
        {
            //ErrorMessage.AddMessage("Couldn't find node, letting default behaviour take over for a frame");
            return base.UpdateTargetPosition(out target);
        }

        if ((Drone.transform.position - currentLootTarget.Position).sqrMagnitude <= maxGrabLootDist * maxGrabLootDist)
        {
            currentLootTarget.Collect(Inventory.main.container);
            currentLootTarget = null;
        }

        target = currentLootTarget.Position;
        return true;
    }
    private CollectableLoot FindTarget()
    {
        var types = new List<TechType>()
        {
            TechType.SandstoneChunk,
            TechType.ShaleChunk,
            TechType.LimestoneChunk,
        };
        //ResourceTrackerDatabase.GetTechTypesInRange(Drone.LeashPosition, maxCollectDist, types);

        var nodes = new List<ResourceTrackerDatabase.ResourceInfo>();
        foreach(var type in types)
        {
            ResourceTrackerDatabase.GetNodes(Drone.LeashPosition, maxCollectDist, type, nodes);
        }

        nodes = nodes.OrderBy(node => (node.position - Drone.transform.position).sqrMagnitude).ToList();

        foreach(var node in nodes)
        {
            if (!NodeIsValid(node))
                continue;

            foreach (var hit in Physics.SphereCastAll(node.position, 0.1f, Vector3.up))
            {
                var BR = hit.collider.GetComponentInParent<BreakableResource>();
                if (BR) return new CollectableLoot(BR);

                var pickable = hit.collider.GetComponentInParent<Pickupable>();
                if (pickable) return new CollectableLoot(pickable);
            }
        }

        return null;
    }
    private bool NodeIsValid(ResourceTrackerDatabase.ResourceInfo node)
    {
        var dif = node.position - Drone.transform.position;
        var dist = dif.magnitude;

        if(dist > maxCollectDist) return false;

        return !Physics.Raycast(Drone.transform.position, dif.normalized, dist - 2);//Nothing between drone and node
    }

    internal class CollectableLoot
    {
        private BreakableResource breakableResource;
        private Pickupable pickupable;
        public CollectableLoot(Pickupable pickupable)
        {
            if (!pickupable) throw new ArgumentNullException("pickupable", "Constructor argument can't be null, needs either resource or pickupable");
            this.pickupable = pickupable;
        }

        public CollectableLoot(BreakableResource breakableResource)
        {
            if (!breakableResource) throw new ArgumentNullException("breakableResource", "Constructor argument can't be null, needs either resource or pickupable");
            this.breakableResource = breakableResource;
        }
        public Vector3 Position => breakableResource ? breakableResource.transform.position : (pickupable ? pickupable.transform.position : throw new Exception("Pickupable and breakable both null on position getting"));
        public void Collect(IItemsContainer inventory)
        {
            ErrorMessage.AddMessage($"Collecting resource {(breakableResource ? breakableResource : pickupable)} and adding to inv {inventory}");
            if(breakableResource)
            {
                breakableResource.BreakIntoResources();
                AddResourceToInventory.resourcesToInventories.Add(breakableResource.transform.position + breakableResource.transform.up * breakableResource.verticalSpawnOffset, inventory);
                return;
            }

            inventory.AddItem(new InventoryItem(pickupable));
        }

    }
}
