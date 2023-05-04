using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetDrawer : MonoBehaviour
{
    public Transform target;
    private LineRenderer lineRenderer;
    public BetterTelloManager betterTelloManager;
    
    Color lineColor= new Color32(117, 184, 255, 255);
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        betterTelloManager = GameObject.Find("Drone").GetComponent<BetterTelloManager>();

        Material mat = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
        lineRenderer.material = mat;
        lineRenderer.startColor = lineColor;
        lineRenderer.endColor = lineColor;
    }
    
    void FixedUpdate()
    {
        if (Vector3.Distance(betterTelloManager.GetComponent<Transform>().position, transform.position) <= 10)
        {
            betterTelloManager.RemoveTarget(this.gameObject);
            return;
        }
        if (target == null) {
            lineRenderer.positionCount = 0;
            return;
        } else
        {
            lineRenderer.positionCount = 2;
        }
       

        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, target.position);
    }
}
