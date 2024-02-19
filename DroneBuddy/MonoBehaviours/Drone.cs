using DroneBuddy.MonoBehaviours.DroneBehaviours;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DroneBuddy.MonoBehaviours;

public class Drone : HandTarget, IHandTarget
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
    public DroneContainer ItemsContainer { get; private set; }
    public DroneName DroneName { get; private set; }
    private IDroneBehaviour currentBehaviour;
    private void Awake()
    {
        behaviours = [.. GetComponents<IDroneBehaviour>()];
        DroneMovement = gameObject.EnsureComponent<DroneMovement>();
        DroneName = gameObject.EnsureComponent<DroneName>();
        UpdateBehaviour();
        ItemsContainer = gameObject.EnsureComponent<DroneContainer>();
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.F)) SetDroneMode(DroneMode == Mode.Far ? Mode.Close : Mode.Far);
        LeashPosition = Player.main?.transform?.position ?? transform.position;

        if (DroneMode == Mode.Bitch) ErrorMessage.AddMessage("Bitch");

        if(currentBehaviour?.UpdateTargetPosition(out var pos) == true)
            DroneMovement.SetTargetPosition(pos);
    }
    private void UpdateBehaviour()
    {
        currentBehaviour?.OnBehaviourEnd();
        currentBehaviour = behaviours.FirstOrDefault(behv => behv.BehaviourMode == DroneMode);
        if (currentBehaviour != null) currentBehaviour.OnBehaviourBegin();
    }

    public void SetDroneMode(Mode droneMode)
    {
        ErrorMessage.AddMessage($"Now setting drone to {droneMode}");
        DroneMode = droneMode;
        UpdateBehaviour();
    }

    public void OnHandHover(GUIHand hand)
    {
        HandReticle.main.SetText(HandReticle.TextType.Hand, $"{DroneName.Name}", true);
    }

    public void OnHandClick(GUIHand hand)
    {
        ItemsContainer.Open();
    }
}
