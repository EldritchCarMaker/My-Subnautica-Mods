using UnityEngine;

namespace DroneBuddy.MonoBehaviours;

internal class DroneCreature : Creature
{
    public enum Mode
    {
        Close,
        Far,
        Bitch
    }
    public Mode DroneMode { get; private set; }
    private void Update()
    {
        if(Input.GetKey(KeyCode.F)) DroneMode = DroneMode == Mode.Far ? Mode.Close : Mode.Far;
        leashPosition = Player.main.transform.position;

        if (DroneMode == Mode.Bitch) ErrorMessage.AddMessage("Bitch");
    }

    public void SetDroneMode(Mode droneMode) => DroneMode = droneMode;
}
