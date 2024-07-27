using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPointTrack : MonoBehaviour
{
    public Transform[] points;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        points = GetComponentsInChildren<Transform>();

        // ù ��° ���(�θ�)�� �ǳʶٰ� �迭�� �ʱ�ȭ
        List<Transform> pointList = new List<Transform>(points);
        pointList.RemoveAt(0);
        points = pointList.ToArray();

        if (points.Length < 2) return;

        Vector3 currentPos = points[0].position;
        Vector3 nextPos;

        for (int i = 1; i <= points.Length; i++)
        {
            nextPos = (i >= points.Length) ? points[0].position : points[i].position;
            Gizmos.DrawLine(currentPos, nextPos);
            currentPos = nextPos;
        }
    }
}
