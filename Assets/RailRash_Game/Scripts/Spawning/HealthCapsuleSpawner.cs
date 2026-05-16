using UnityEngine;
using UnityEngine.Splines;

public class HealthCapsuleSpawner : MonoBehaviour
{
    [Header("Spline")]
    public SplineContainer splineContainer;

    [Header("Health Capsule")]
    public GameObject healthCapsulePrefab;

    [Header("Lane")]
    public float laneDistance = 2f;

    [Header("Capsule Height")]
    public float heightOffset = 1f;

    [Header("Spawn Settings")]
    public float startProgress = 0.2f;

    // Large distance between capsules
    public float capsuleGap = 0.30f;

    void Start()
    {
        SpawnCapsules();
    }

    void SpawnCapsules()
    {
        if (splineContainer == null)
        {
            Debug.LogError("Spline missing!");
            return;
        }

        if (healthCapsulePrefab == null)
        {
            Debug.LogError("Health capsule missing!");
            return;
        }

        for (float t = startProgress; t < 1f; t += capsuleGap)
        {
            Vector3 splinePos =
                splineContainer.EvaluatePosition(t);

            Vector3 tangent =
                ((Vector3)splineContainer
                .EvaluateTangent(t)).normalized;

            Vector3 right =
                Vector3.Cross(Vector3.up, tangent)
                .normalized;

            int lane =
                Random.Range(0, 3);

            float laneOffset =
                (lane - 1) * laneDistance;

            Vector3 spawnPos =
                splinePos +
                right * laneOffset +
                Vector3.up * heightOffset;

            Quaternion rot =
                Quaternion.LookRotation(tangent);

            Instantiate(
                healthCapsulePrefab,
                spawnPos,
                rot
            );
        }
    }
}