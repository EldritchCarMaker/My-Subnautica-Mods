using UnityEngine;

namespace DroneBuddy.MonoBehaviours.DroneBehaviours;

public interface IDroneBehaviour
{
    public void OnBehaviourBegin();
    public void OnBehaviourEnd();

    public Drone.Mode BehaviourMode { get; }
}
public abstract class DroneBehaviour : MonoBehaviour, IDroneBehaviour
{
    protected Drone Drone {  get; private set; }
    public abstract Drone.Mode BehaviourMode { get; }

    protected bool isActive = false;
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
        ErrorMessage.AddMessage($"Beginning {this} behaviour!");
    }

    private void Update()
    {
        if (isActive) ActiveUpdate();
    }

    protected virtual void ActiveUpdate()
    {

    }

    public virtual void OnBehaviourEnd()
    {
        isActive = false;
        ErrorMessage.AddMessage($"Ending {this} behaviour!");
    }
}

