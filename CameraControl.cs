using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [Header("Targets")]
    [SerializeField] Transform Target;
    [SerializeField] Camera Camera;

    [Header("Settings")]
    [SerializeField] float smoothTime;
    [SerializeField] float sizeChangeSpeed;

    [Header("Waypoints")]
    [SerializeField] List<CameraSetting> pathPoints = new List<CameraSetting>();

    private int currentPointIndex = 0;
    private Vector3 velocity = Vector3.zero;

    private void LateUpdate() 
    {
        if (pathPoints.Count > 0 && Target != null)
        {
           Vector2 targetPosition = new Vector2(Target.position.x, Target.position.y);
           int newIndex = currentPointIndex;
           float closestDistance = float.MaxValue;

           for (int i = 0; i < pathPoints.Count; i++)
            {
                float distance = Vector2.Distance(targetPosition, pathPoints[i].position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    newIndex = i;
                }
            }

            if (newIndex != currentPointIndex)
            {
                if (Mathf.Abs(newIndex - currentPointIndex) == 1)
                {
                    currentPointIndex = newIndex;
                }
            }

            Vector3 cameraTargetPosition = new Vector3(Target.position.x, CalculateYPosition(currentPointIndex), Camera.transform.position.z);
            Camera.transform.position = Vector3.SmoothDamp(Camera.transform.position, cameraTargetPosition, ref velocity, smoothTime);
            Camera.orthographicSize = Mathf.Lerp(Camera.orthographicSize, pathPoints[currentPointIndex].size, Time.deltaTime * sizeChangeSpeed);
        }
    }

    private float CalculateYPosition(int index)
    {
        if (pathPoints.Count < 2) return Camera.transform.position.y;

        if (index < pathPoints.Count - 1)
        {
            float t = Mathf.InverseLerp(pathPoints[index].position.x, pathPoints[index + 1].position.x, Target.position.x);
            return Mathf.Lerp(pathPoints[index].position.y, pathPoints[index + 1].position.y, t);
        }
        return pathPoints[index].position.y;
    }

    private void OnDrawGizmos()
    {
        if (pathPoints.Count == 0) return;

        Gizmos.color = Color.green;
        for (int i = 0; i < pathPoints.Count - 1; i++)
        {
            Vector3 start = new Vector3(pathPoints[i].position.x, pathPoints[i].position.y, 0);
            Vector3 end = new Vector3(pathPoints[i + 1].position.x, pathPoints[i + 1].position.y, 0);
            Gizmos.DrawLine(start, end);
        }

        Gizmos.color = Color.red;
        foreach (CameraSetting point in pathPoints)
        {
            Gizmos.DrawSphere(new Vector3(point.position.x, point.position.y, 0), 0.1f);
        }
    }

}


[System.Serializable]
public class CameraSetting
{
    public Vector2 position;
    public float size;
}
