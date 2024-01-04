using UnityEngine;

namespace DroneBuddy.MonoBehaviours.DroneBehaviours;

internal class DroneFollowClose : DroneBehaviour
{
    private static float timeRandomMove = 10;
    private float timeLastRandomMove = timeRandomMove;

    public override Drone.Mode BehaviourMode => Drone.Mode.Close;

    public override bool UpdateTargetPosition(out Vector3 target)
    {
        timeLastRandomMove -= Time.deltaTime;

        if (timeLastRandomMove <= 0)
        {
            timeLastRandomMove = timeRandomMove;
            ErrorMessage.AddMessage("Moving random!");
            target = Drone.LeashPosition + (Random.insideUnitSphere * (maxFollowDist - 2));
            return true;
        }
        else
        {
            return base.UpdateTargetPosition(out target);
        }
    }
}
