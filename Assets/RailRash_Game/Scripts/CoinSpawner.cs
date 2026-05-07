using System.Collections.Generic;
using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    public GameObject coinPrefab;
    public float sideOffset = 2f;
    public float heightOffset = 1f;
    public float waveInterval = 50f;

    private List<Transform> pathPoints = new List<Transform>();

    void Start()
    {
        GetPathPoints();
        SpawnCoins();
    }

    void GetPathPoints()
    {
        pathPoints.Clear();
        foreach (Transform child in transform)
            if (child.name.Contains("Point"))
                pathPoints.Add(child);

        // Natural sort so Point2 < Point10 (not Point1 < Point10 < Point2)
        pathPoints.Sort((a, b) =>
        {
            string na = a.name.Replace("Point", "");
            string nb = b.name.Replace("Point", "");
            if (int.TryParse(na, out int ia) && int.TryParse(nb, out int ib))
                return ia.CompareTo(ib);
            return string.CompareOrdinal(a.name, b.name);
        });
    }

    // ── Walk path by real world distance.
    //    First wave fires after waveInterval metres — NOT at game start. ──
    void SpawnCoins()
    {
        if (pathPoints.Count < 2) return;

        float travelled = 0f;
        float nextWaveAt = waveInterval;   // skip distance 0 → no coins on screen at start

        for (int i = 0; i < pathPoints.Count - 1; i++)
        {
            float segLen = Vector3.Distance(
                pathPoints[i].position, pathPoints[i + 1].position);

            // A segment can contain more than one wave trigger
            while (nextWaveAt <= travelled + segLen)
            {
                float tStart = (nextWaveAt - travelled) / segLen;
                SpawnCoinWave(i, Mathf.Clamp01(tStart));
                nextWaveAt += waveInterval;
            }

            travelled += segLen;
        }
    }

    // ── Smooth tangent: Slerp between prev/curr/next segment directions ──
    Vector3 GetForward(int idx, float t)
    {
        Vector3 curr = Dir(idx, idx + 1);
        Vector3 prev = idx > 0 ? Dir(idx - 1, idx) : curr;
        Vector3 next = idx + 2 < pathPoints.Count ? Dir(idx + 1, idx + 2) : curr;

        Vector3 fwd = Vector3.Slerp(
                          Vector3.Slerp(prev, curr, 0.5f),
                          Vector3.Slerp(curr, next, 0.5f),
                          t);

        fwd = Vector3.ProjectOnPlane(fwd, Vector3.up);
        return fwd.sqrMagnitude > 0.001f ? fwd.normalized : curr;
    }

    Vector3 Dir(int from, int to) =>
        (pathPoints[to].position - pathPoints[from].position).normalized;

    // 0 = straight road, 1 = 90-degree corner
    float Curvature(int idx)
    {
        if (idx <= 0 || idx >= pathPoints.Count - 1) return 0f;
        return Mathf.Clamp01(Vector3.Angle(Dir(idx - 1, idx), Dir(idx, idx + 1)) / 90f);
    }

    void SpawnCoinWave(int startIdx, float startT)
    {
        int coinCount = Random.Range(4, 6);
        float spacing = 2f;
        int lane = Random.Range(-1, 2);

        int idx = startIdx;
        float t = startT;

        for (int i = 0; i < coinCount; i++)
        {
            if (idx >= pathPoints.Count - 1) break;

            // Place first coin exactly at trigger; advance by spacing afterwards
            if (i > 0)
            {
                float move = spacing;
                while (move > 0f && idx < pathPoints.Count - 1)
                {
                    Transform a2 = pathPoints[idx];
                    Transform b2 = pathPoints[idx + 1];
                    float seg = Vector3.Distance(a2.position, b2.position);
                    float rem = seg * (1f - t);

                    if (move <= rem) { t += move / seg; move = 0f; }
                    else { move -= rem; idx++; t = 0f; }
                }
                if (idx >= pathPoints.Count - 1) break;
            }

            // ── compute final position ──
            Vector3 pos = Vector3.Lerp(pathPoints[idx].position,
                                           pathPoints[idx + 1].position, t);
            Vector3 forward = GetForward(idx, t);
            Vector3 right = Vector3.Cross(Vector3.up, forward).normalized;

            // Shrink offset on corners so coins never leave the road surface
            float curv = Mathf.Lerp(Curvature(idx), Curvature(idx + 1), t);
            float offset = sideOffset * (1f - curv * 0.9f);

            Vector3 spawn = pos + right * (lane * offset);
            spawn.y += heightOffset;

            Instantiate(coinPrefab, spawn, Quaternion.LookRotation(forward, Vector3.up));
        }
    }
}