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

    [Header("Bomb Settings")]
    public float bombExtraHeight = 1f;

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

            // Lane offset
            float laneOffset =
                (lane - 1) * laneDistance;

            // Random obstacle
            int randomIndex =
                Random.Range(0, obstaclePrefabs.Length);

            GameObject obstacle =
                obstaclePrefabs[randomIndex];

            // Default height
            float finalHeight = heightOffset;

            // Bomb gets extra height
            if (obstacle.name.Contains("Bomb"))
            {
                finalHeight += bombExtraHeight;
            }

            if (obstacle.name.Contains("obstacle poisonthrower"))
            {
                finalHeight += 2;
            }

            if (obstacle.name.Contains("obstacle flamethrower 1"))
            {
                finalHeight += 2;
            }

            // Final position
            Vector3 spawnPos =
                splinePos +
                right * laneOffset +
                Vector3.up * finalHeight;

            // Rotation
            Quaternion rot =
                Quaternion.LookRotation(tangent);

            // Spawn
            Instantiate(obstacle, spawnPos, rot);
        }
    }
}