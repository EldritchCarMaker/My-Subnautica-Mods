using UnityEngine;

namespace DroneBuddy.MonoBehaviours.DroneBehaviours;

internal class DroneFollowClose : DroneBehaviour
{
    private static float maxFollowDist = 5;
    private static float timeRandomMove = 10;
    private float timeLastRandomMove = timeRandomMove;

    public override Drone.Mode BehaviourMode => Drone.Mode.Close;

    protected override void ActiveUpdate()
    {
        timeLastRandomMove -= Time.deltaTime;

        var dif = (Drone.DroneMovement.TargetPosition - Drone.LeashPosition);

        if (timeLastRandomMove <= 0)
        {
            timeLastRandomMove = timeRandomMove;
            ErrorMessage.AddMessage("Moving random!");
            Drone.DroneMovement.SetTargetPosition(Drone.LeashPosition + (Random.insideUnitSphere * (maxFollowDist - 2)));
        }

        else if(dif.sqrMagnitude >= maxFollowDist*maxFollowDist)
        {
            ErrorMessage.AddMessage("Too far! Coming back");

            //var pos = Drone.LeashPosition + (Random.insideUnitSphere * (maxFollowDist - 2));//Random
            var pos = Drone.LeashPosition + (3.5f * dif.normalized);//Closest spot to drone

            Drone.DroneMovement.SetTargetPosition(pos);
        }
    }
}
