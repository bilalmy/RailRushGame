using UnityEngine;
using UnityEngine.Splines;

public class CoinSpawner : MonoBehaviour
{
    [Header("Spline")]
    public SplineContainer splineContainer;

    [Header("Coin")]
    public GameObject coinPrefab;

    [Header("Lane Settings")]
    public float laneDistance = 2f;

    [Header("Height")]
    public float heightOffset = 1f;

    [Header("Spawn Settings")]
    [Range(0f, 1f)]
    public float startProgress = 0.1f;

    // Distance between each coin inside a collection
    public float gapBetweenCoins = 0.01f;

    // Distance between collections
    public float gapBetweenCollections = 0.08f;

    // How many coins in one collection
    public int coinsPerCollection = 5;

    void Start()
    {
        SpawnCoins();
    }

    void SpawnCoins()
    {
        if (splineContainer == null || coinPrefab == null)
        {
            Debug.LogError("Missing SplineContainer or CoinPrefab!");
            return;
        }

        // Move along spline collection by collection
        for (float t = startProgress; t < 1f; t += gapBetweenCollections)
        {
            // Random lane for whole collection
            int lane = Random.Range(0, 3);

            // Convert lane to offset
            float laneOffset = (lane - 1) * laneDistance;

            // Spawn 5 coins in one collection
            for (int i = 0; i < coinsPerCollection; i++)
            {
                float currentT = t + (i * gapBetweenCoins);

                // Prevent going outside spline range
                if (currentT > 1f)
                    break;

                // Position on spline
                Vector3 splinePos =
                    splineContainer.EvaluatePosition(currentT);

                // Forward direction
                Vector3 tangent =
                    ((Vector3)splineContainer.EvaluateTangent(currentT)).normalized;

                // Right direction
                Vector3 right =
                    Vector3.Cross(Vector3.up, tangent).normalized;

                // Final spawn position
                Vector3 spawnPos =
                    splinePos +
                    right * laneOffset +
                    Vector3.up * heightOffset;

                // Rotation facing forward
                Quaternion rot =
                    Quaternion.LookRotation(tangent);

                // Spawn coin
                Instantiate(coinPrefab, spawnPos, rot);
            }
        }
    }
}