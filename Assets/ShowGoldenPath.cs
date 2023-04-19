using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.PackageManager;
// ShowGoldenPath
using UnityEngine;
using UnityEngine.AI;

public class ShowGoldenPath : MonoBehaviour
{
    public float targetY;
    public bool IsTargetReachable;
    public NavMeshPathStatus status;
    Transform target;
    private float droneY;
    private NavMeshPath path;
    private float elapsed = 0.0f;
    private LineRenderer lineRenderer;
    private BetterTelloManager betterTelloManager;


    public class CalculatedGoldenPathEventArgs : EventArgs
    {

    }
    public event EventHandler<CalculatedGoldenPathEventArgs> OnCalculatedGoldenPath;
    
    void Start()
    {
        path = new NavMeshPath();
        elapsed = 0.0f;
        lineRenderer = GetComponent<LineRenderer>();
        betterTelloManager = GameObject.Find("Drone").GetComponent<BetterTelloManager>();
    }

    public float GetDistanceToTargetTransform() => target != null ? Vector3.Distance(transform.position, target.position) : -1f;

    void FixedUpdate()
    {
        target = betterTelloManager.GetNextTarget()?.GetComponent<Transform>();
        if (target == null) {
            lineRenderer.positionCount = 0;
            return;
        }

        droneY = transform.rotation.eulerAngles.y;
        if (path.corners.Length > 0)
        {
            var gmo = new GameObject();
            var newTrans = gmo.transform;
            newTrans.SetPositionAndRotation(transform.position, transform.rotation);
            newTrans.LookAt(path.corners[1]);
            targetY = AngleDifference(transform.rotation.eulerAngles.y, newTrans.rotation.eulerAngles.y);
            Destroy(gmo);
        }
        else
            targetY = droneY;
        if (elapsed > 0.01f)
        {
            IsTargetReachable = NavMesh.CalculatePath(transform.position, target.position, NavMesh.AllAreas, path);
            status = path.status;
            elapsed -= 0.01f;
        }
        lineRenderer.positionCount = path.corners.Length;
        for (int i = 0; i < path.corners.Length - 1; i++)
        {
            Debug.DrawLine(path.corners[i], path.corners[i + 1], Color.red);
            lineRenderer.SetPosition(i, path.corners[i]);
            lineRenderer.SetPosition(i + 1, path.corners[i + 1]);
        }

        elapsed += Time.deltaTime;
    }

    public float AngleDifference(float angle1, float angle2)
    {
        float diff = (angle2 - angle1 + 180) % 360 - 180;
        return diff < -180f ? diff + 360f : diff;
    }
}
