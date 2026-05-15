using UnityEngine;
using UnityEngine.Splines;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Spline")]
    public SplineContainer splineContainer;

    [Header("Obstacle Prefabs")]
    public GameObject[] obstaclePrefabs;

    [Header("Lane Settings")]
    public float laneDistance = 2f;

    [Header("Height")]
    public float heightOffset = 0.5f;

    [Header("Spawn Settings")]
    [Range(0f, 1f)]
    public float startProgress = 0.1f;

    // Distance between obstacles
    public float gapBetweenObstacles = 0.08f;

    // Chance to spawn obstacle
    [Range(0f, 100f)]
    public float spawnChance = 70f;

    void Start()
    {
        SpawnObstacles();
    }

    void SpawnObstacles()
    {
        // Safety checks
        if (splineContainer == null)
        {
            Debug.LogError("Spline Container Missing!");
            return;
        }

        if (obstaclePrefabs.Length == 0)
        {
            Debug.LogError("No obstacle prefabs added!");
            return;
        }

        // Move along spline
        for (float t = startProgress; t < 1f; t += gapBetweenObstacles)
        {
            // Random chance
            float randomChance = Random.Range(0f, 100f);

            // Skip spawn if chance fails
            if (randomChance > spawnChance)
                continue;

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

            // Final position
            Vector3 spawnPos =
                splinePos +
                right * laneOffset +
                Vector3.up * heightOffset;

            // Rotation
            Quaternion rot =
                Quaternion.LookRotation(tangent);

            // Random obstacle prefab
            int randomIndex =
                Random.Range(0, obstaclePrefabs.Length);

            GameObject obstacle =
                obstaclePrefabs[randomIndex];

            // Spawn obstacle
            Instantiate(obstacle, spawnPos, rot);
        }
    }
}