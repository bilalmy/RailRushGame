using UnityEngine;
using System.Collections.Generic;

public class PathManager : MonoBehaviour
{
    public List<Transform> pathPoints = new List<Transform>();

    private void OnDrawGizmos()
    {
        if (pathPoints == null || pathPoints.Count == 0) return;

        Gizmos.color = Color.green;

        for (int i = 0; i < pathPoints.Count; i++)
        {
            if (pathPoints[i] != null)
            {
                Gizmos.DrawSphere(pathPoints[i].position, 0.3f);
            }

            if (i < pathPoints.Count - 1 && pathPoints[i] != null && pathPoints[i + 1] != null)
            {
                Gizmos.DrawLine(pathPoints[i].position, pathPoints[i + 1].position);
            }
        }
    }
}