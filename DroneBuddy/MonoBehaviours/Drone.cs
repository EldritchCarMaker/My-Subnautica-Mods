using DroneBuddy.MonoBehaviours.DroneBehaviours;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DroneBuddy.MonoBehaviours;

public class Drone : MonoBehaviour
{
    private List<IDroneBehaviour> behaviours = new();
    public enum Mode
    {
        Close,
        Far,
        Bitch
    }
    public Mode DroneMode { get; private set; } = Mode.Close;
    public Vector3 LeashPosition {  get; private set; }
    public DroneMovement DroneMovement { get; private set; }
    private IDroneBehaviour currentBehaviour;
    private void Awake()
    {
        behaviours = [.. GetComponents<IDroneBehaviour>()];
        DroneMovement = gameObject.EnsureComponent<DroneMovement>();
        ChangeBehaviour();
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.F)) DroneMode = DroneMode == Mode.Far ? Mode.Close : Mode.Far;
        LeashPosition = Player.main?.transform?.position ?? transform.position;

        if (DroneMode == Mode.Bitch) ErrorMessage.AddMessage("Bitch");
    }
    private void ChangeBehaviour()
    {
        currentBehaviour?.OnBehaviourEnd();
        behaviours.FirstOrDefault(behv => behv.BehaviourMode == DroneMode)?.OnBehaviourBegin();
    }

    public void SetDroneMode(Mode droneMode)
    {
        DroneMode = droneMode;
        ChangeBehaviour();
    }
}
