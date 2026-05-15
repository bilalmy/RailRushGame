using UnityEngine;
using UnityEngine.Splines;

public class BombSpawner : MonoBehaviour
{
    [Header("Spline")]
    public SplineContainer splineContainer;

    [Header("Bomb")]
    public GameObject bombPrefab;

    [Header("Lane Settings")]
    public float laneDistance = 2f;

    [Header("Height")]
    public float heightOffset = 1f;

    [Header("Spawn Settings")]
    [Range(0f, 1f)]
    public float startProgress = 0.1f;

    public float gapBetweenBombs = 0.05f;

    void Start()
    {
        SpawnBombs();
    }

    void SpawnBombs()
    {
        if (splineContainer == null || bombPrefab == null)
        {
            Debug.LogError("Missing SplineContainer or BombPrefab!");
            return;
        }

        // Move along spline
        for (float t = startProgress; t < 1f; t += gapBetweenBombs)
        {
            // Position on spline
            Vector3 splinePos =
                splineContainer.EvaluatePosition(t);

            // Forward direction
            Vector3 tangent =
                ((Vector3)splineContainer.EvaluateTangent(t)).normalized;

            // Right direction
            Vector3 right =
                Vector3.Cross(Vector3.up, tangent).normalized;

            // Random lane
            int lane = Random.Range(0, 3);

            // Convert lane to offset
            float laneOffset =
                (lane - 1) * laneDistance;

            // Final spawn position
            Vector3 spawnPos =
                splinePos +
                right * laneOffset +
                Vector3.up * heightOffset;

            // Rotation facing forward
            Quaternion rot =
                Quaternion.LookRotation(tangent);

            // Spawn bomb
            Instantiate(bombPrefab, spawnPos, rot);
        }
    }
}