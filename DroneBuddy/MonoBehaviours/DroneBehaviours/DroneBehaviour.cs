using UnityEngine;
using static VoxelandChunk;

namespace DroneBuddy.MonoBehaviours.DroneBehaviours;

public interface IDroneBehaviour
{
    public void OnBehaviourBegin();
    public void OnBehaviourEnd();
    public bool UpdateTargetPosition(out Vector3 target);

    public Drone.Mode BehaviourMode { get; }
}
public abstract class DroneBehaviour : MonoBehaviour, IDroneBehaviour
{
    protected Drone Drone {  get; private set; }
    public abstract Drone.Mode BehaviourMode { get; }

    protected bool isActive = false;
    protected virtual float maxFollowDist => 5;
    private void Awake()
    {
        Drone = GetComponent<Drone>();
        OnAwake();
    }
    protected virtual void OnAwake()
    {

    }
    public virtual void OnBehaviourBegin()
    {
        isActive = true;
        //ErrorMessage.AddMessage($"Beginning {this} behaviour!");
    }

    private void Update()
    {
        if (isActive) ActiveUpdate();
    }

    protected virtual void ActiveUpdate()
    {

    }

    public virtual bool UpdateTargetPosition(out Vector3 target)
    {
        var dif = (Drone.DroneMovement.TargetPosition - Drone.LeashPosition);

        if (dif.sqrMagnitude >= maxFollowDist * maxFollowDist)
        {
            //ErrorMessage.AddMessage("Too far! Coming back");

            var pos = Drone.LeashPosition + (3.5f * dif.normalized);//Closest spot to drone

            target = pos;
            return true;
        }

        target = default;
        return false;
    }

    public virtual void OnBehaviourEnd()
    {
        isActive = false;
        //ErrorMessage.AddMessage($"Ending {this} behaviour!");
    }
}

