using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;

    [Header("Position Settings")]
    public float height = 5f;          // Camera height
    public float distance = 15f;       // Distance behind player

    [Header("Smooth Settings")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;

    void LateUpdate()
    {
        if (player == null) return;

        // Calculate position behind player based on forward direction
        Vector3 desiredPosition = player.position
                                - player.forward * distance
                                + Vector3.up * height;

        // Smooth position movement
        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            moveSpeed * Time.deltaTime
        );

        // Calculate rotation to match player's forward direction
        Quaternion desiredRotation = Quaternion.LookRotation(player.forward);

        // Smooth rotation
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            desiredRotation,
            rotationSpeed * Time.deltaTime
        );
    }
}