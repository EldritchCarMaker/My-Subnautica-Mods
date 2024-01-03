using UnityEngine;

namespace DroneBuddy.MonoBehaviours;

public class DroneMovement : MonoBehaviour
{
    public Vector3 TargetPosition { get; private set; }
    public void SetTargetPosition(Vector3 targetPosition)
    {
        ErrorMessage.AddMessage($"Setting target position to {targetPosition}");
        TargetPosition = targetPosition;
    }

    private Rigidbody rb;
    private static float minDistToMove = 1f;
    private static float distMaxSpeed = 50;
    private static float moveSpeed = 10f;
    private static float maxMoveSpeed = 30f;
    private static float turnSpeed = 2.5f;
    private void Awake() => rb = GetComponent<Rigidbody>();
    private void FixedUpdate()
    {
        var dif = TargetPosition - transform.position;

        if (dif.sqrMagnitude <= minDistToMove * minDistToMove) return;

        transform.forward = Vector3.Slerp(transform.forward, dif.normalized, turnSpeed * Time.deltaTime);
        rb.AddForce(dif.normalized * GetMoveSpeed(dif.sqrMagnitude), ForceMode.Acceleration);
    }
    public float GetMoveSpeed(float sqrDist)
    {
        return Mathf.Lerp(moveSpeed, maxMoveSpeed, sqrDist / (distMaxSpeed * distMaxSpeed));
    }
}
