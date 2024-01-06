using DroneBuddy.Patches;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

namespace DroneBuddy.MonoBehaviours.DroneBehaviours;

public abstract class SeekerBehaviour : DroneBehaviour
{
    private Component currentLootTarget;
    protected virtual float maxCollectDist => 30;
    protected virtual float maxGrabLootDist => 2;
    protected abstract Type[] ComponentTypesToFind { get; }
    protected abstract TechType[] TechTypesToSeek { get; }
    public override void OnBehaviourEnd()
    {
        currentLootTarget = null;
    }
    public override bool UpdateTargetPosition(out Vector3 target)
    {
        if (currentLootTarget == null)
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

        if ((Drone.transform.position - currentLootTarget.transform.position).sqrMagnitude <= maxGrabLootDist * maxGrabLootDist)
        {
            Collect(currentLootTarget, Inventory.main.container);
            currentLootTarget = null;
        }

        target = currentLootTarget.transform.position;
        return true;
    }
    protected abstract void Collect(Component lootTarget, IItemsContainer inventory);
    protected Component FindTarget()
    {


        var nodes = new List<ResourceTrackerDatabase.ResourceInfo>();
        foreach (var type in TechTypesToSeek)
        {
            ResourceTrackerDatabase.GetNodes(Drone.LeashPosition, maxCollectDist, type, nodes);
        }

        nodes = nodes.OrderBy(node => (node.position - Drone.transform.position).sqrMagnitude).ToList();

        foreach (var node in nodes)
        {
            if (!NodePositionIsValid(node))
                continue;

            foreach (var hit in Physics.SphereCastAll(node.position, 0.1f, Vector3.up))
            {
                var rb = hit.rigidbody;
                var obj = rb ? rb.gameObject : hit.collider.gameObject;

                if(GameObjectIsTarget(obj, out var targetComp))
                    return targetComp;
            }
        }

        return null;
    }
    protected virtual bool GameObjectIsTarget(GameObject go, out Component targetComp)
    {
        foreach(var type in ComponentTypesToFind)
        {
            if(go.TryGetComponent(type, out targetComp))
                return true;
        }
        targetComp = null;
        return false;
    }
    protected virtual bool NodePositionIsValid(ResourceTrackerDatabase.ResourceInfo node)
    {
        var dif = node.position - Drone.transform.position;
        var dist = dif.magnitude;

        if (dist > maxCollectDist) return false;

        return !Physics.Raycast(Drone.transform.position, dif.normalized, dist - 2);//Nothing between drone and node
    }
}
