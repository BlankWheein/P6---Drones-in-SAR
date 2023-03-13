using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
// ShowGoldenPath
using UnityEngine;
using UnityEngine.AI;

public class ShowGoldenPath : MonoBehaviour
{
    public Transform target;
    public float targetY;

    private float droneY;
    private NavMeshPath path;
    private float elapsed = 0.0f;
    private LineRenderer lineRenderer;

    public class CalculatedGoldenPathEventArgs : EventArgs
    {

    }
    public event EventHandler<CalculatedGoldenPathEventArgs> OnCalculatedGoldenPath;
    
    void Start()
    {
        path = new NavMeshPath();
        elapsed = 0.0f;
        lineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        droneY = transform.rotation.eulerAngles.y;
        if (path.corners.Length > 0)
        {
            var gmo = new GameObject();
            var newTrans = gmo.transform;
            newTrans.SetPositionAndRotation(transform.position, transform.rotation);
            newTrans.LookAt(path.corners[1]);
            if (newTrans.rotation.eulerAngles.y < 180)
                targetY = newTrans.rotation.eulerAngles.y - droneY;
            else
                targetY = newTrans.rotation.eulerAngles.y - 360 - droneY;
            Destroy(gmo);
        } else
        {
            targetY = droneY;
        }
        elapsed += Time.deltaTime;
        if (elapsed > 0.01f)
        {
            Debug.Log(target.position);
            NavMesh.CalculatePath(transform.position, target.position, NavMesh.AllAreas, path);
            elapsed -= 0.01f;
        }
        lineRenderer.positionCount = path.corners.Length;
        for (int i = 0; i < path.corners.Length - 1; i++)
        {
            Debug.DrawLine(path.corners[i], path.corners[i + 1], Color.red);
            lineRenderer.SetPosition(i, path.corners[i]);
            lineRenderer.SetPosition(i + 1, path.corners[i + 1]);
        }
    }
}
