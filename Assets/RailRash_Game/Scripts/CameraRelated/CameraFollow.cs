using Unity.VisualScripting;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;

    [Header("Position Settings")]
    public float height = 5f;
    public float distance = 10f;
    public float sideOffset = 0f; // slight sideways shift

    [Header("Smooth Settings")]
    public float moveSpeed = 8f;
    public float rotationSpeed = 10f;

    void LateUpdate()
    {
        if (player == null) return;

        // forward → where player is facing
        //right → sideways direction of player
        Vector3 forward = player.forward;
        Vector3 right = player.right;

        // Target position (behind + up + slight side)
        Vector3 targetPos = player.position
                          - forward * distance
                          + Vector3.up * height
                          + right * sideOffset;

        // Smooth move
        transform.position = Vector3.Lerp(
            transform.position,
            targetPos,
            moveSpeed * Time.deltaTime
        );

        // Look at player
        Quaternion lookRot = Quaternion.LookRotation(player.position - transform.position);

        // Smooth rotation
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            lookRot,
            rotationSpeed * Time.deltaTime
        );
    }
}