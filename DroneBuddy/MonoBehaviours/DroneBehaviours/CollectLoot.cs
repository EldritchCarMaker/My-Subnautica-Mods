using DroneBuddy.Patches;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DroneBuddy.MonoBehaviours.DroneBehaviours;

internal class CollectLoot : SeekerBehaviour
{
    public override Drone.Mode BehaviourMode => Drone.Mode.Far;

    protected override Type[] ComponentTypesToFind => new[] { typeof(BreakableResource), typeof(Pickupable) };

    protected override TechType[] TechTypesToSeek => new[] { TechType.Quartz, TechType.LimestoneChunk, TechType.SandstoneChunk, TechType.ShaleChunk, };

    protected override void Collect(Component lootTarget, IItemsContainer inventory)
    {
        if(lootTarget is BreakableResource resource)
        {
            resource.BreakIntoResources(); 
            AddResourceToInventory.resourcesToInventories.Add(resource.transform.position + resource.transform.up * resource.verticalSpawnOffset, inventory);
            return;
        }
        if(lootTarget is Pickupable pickupable)
        {
            pickupable.Pickup(true);
            inventory.AddItem(new InventoryItem(pickupable));
            return;
        }
        
        throw new ArgumentException($"Loot target was type {lootTarget.GetType()} and not one of the expected types for this seeker behaviour", "lootTarget");
    }
}
