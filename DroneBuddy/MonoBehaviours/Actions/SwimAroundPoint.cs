using UnityEngine;

namespace DroneBuddy.MonoBehaviours.Actions;

internal class SwimAroundPoint : CreatureAction
{
    public float swimDist = 4;
    public override void StartPerform(Creature creature, float time)
    {
        base.StartPerform(creature, time);
        var point = Random.onUnitSphere * 5;
        swimBehaviour.SwimTo(point + creature.leashPosition, swimDist);
    }
}
