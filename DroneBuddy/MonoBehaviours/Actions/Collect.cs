using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ResourceInfo = ResourceTrackerDatabase.ResourceInfo;

namespace DroneBuddy.MonoBehaviours.Actions;

internal class Collect : CreatureAction
{
    private List<ResourceInfo> resources = new();
    private static List<TechType> allowedTypes = new List<TechType>()
    {
        TechType.SandstoneChunk, TechType.LimestoneChunk, TechType.ShaleChunk
    };
    private static float maxDistancePickup = 2;
    private static float maxDistance = 30;
    public override float Evaluate(Creature creature, float time)
    {
        return creature is DroneCreature drone && drone.DroneMode == DroneCreature.Mode.Far ? 1f : 0f;
    }
    public override void StartPerform(Creature creature, float time)
    {
        base.StartPerform(creature, time);
        ErrorMessage.AddMessage("Starting collect");
        Refresh();
    }
    public override void StopPerform(Creature creature, float time)
    {
        base.StopPerform(creature, time);
        ErrorMessage.AddMessage("Stopping collect");
        resources.Clear();
    }

    private void Update()
    {
        if (resources.Count <= 0) return;

        var targPos = resources[0].position;
        var distanceFromDrone = Vector3.Distance(targPos, transform.position);

        //Too far, or don't have LOS to the resource
        if ((targPos - creature.leashPosition).sqrMagnitude > maxDistance * maxDistance || Physics.Raycast(transform.position, targPos - transform.position, distanceFromDrone - 1f))
        {
            resources.RemoveAt(0);
            if (resources.Count == 0)
            {
                Refresh();
                swimBehaviour.SwimTo(creature.leashPosition, 12);//Reset position back to player, make sure drone doesn't freeze in one place trying to grab items
            }
            return;
        }

        if (distanceFromDrone <= maxDistancePickup)
        {
            CollectItem();
            resources.RemoveAt(0);
            if (resources.Count == 0) Refresh();

        }
        else
        {
            swimBehaviour.SwimTo(targPos, 8);
        }
    }

    private void CollectItem()
    {
        var collides = Physics.OverlapSphere(transform.position, maxDistancePickup);
        foreach(var collider in collides)
        {
            var res = collider.GetComponentInParent<BreakableResource>();
            if (!res) continue;

            res.transform.position = creature.leashPosition + (res.transform.position - creature.leashPosition).normalized;
            return;
        }
        ErrorMessage.AddMessage("Could not find loot thingy!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! BITCH");
    }

    private void Refresh()
    {
        var res = new List<ResourceInfo>();
        foreach(var type in allowedTypes)
        {
            ResourceTrackerDatabase.GetNodes(creature.leashPosition, maxDistance, type, res);
        }
        var sorted = res.OrderBy(resource => (resource.position - transform.position).sqrMagnitude);
        resources = [.. sorted];//Neat, just converts to a list
    }
}
