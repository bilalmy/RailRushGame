using System.Collections.Generic;
using UnityEngine;

public class BombSpawner : MonoBehaviour
{
    [Header("References")]
    public PathManager pathManager;
    public GameObject bombPrefab;

    [Header("Lane Settings")]
    public float laneDistance = 2f;

    [Header("Height Settings")]
    public float heightOffset = 2f;

    [Header("Spawn Settings")]
    public int startPoint = 15;
    public int gapBetweenBombs = 25;

    private List<Transform> points;

    void Start()
    {
        // Get all path points from PathManager
        points = pathManager.pathPoints;

        // Spawn bombs
        SpawnBombs();
    }

    void SpawnBombs()
    {
        // Spawn bombs after some gap
        for (int i = startPoint; i < points.Count; i += gapBetweenBombs)
        {
            // Current path point
            Transform point = points[i];

            // Random lane:
            // 0 = Left
            // 1 = Center
            // 2 = Right
            int lane = Random.Range(0, 3);

            // Convert lane number into position offset
            float laneOffset = (lane - 1) * laneDistance;

            // Get road forward direction
            Vector3 forward;

            if (i < points.Count - 1)
            {
                forward = (points[i + 1].position - point.position).normalized;
            }
            else
            {
                forward = (point.position - points[i - 1].position).normalized;
            }

            // Get right direction of road
            Vector3 right = Vector3.Cross(Vector3.up, forward);

            // Final bomb position
            Vector3 spawnPos =
                point.position +
                right * laneOffset +
                Vector3.up * heightOffset;

            // Spawn bomb
            Instantiate(bombPrefab, spawnPos, Quaternion.identity);
        }
    }
}