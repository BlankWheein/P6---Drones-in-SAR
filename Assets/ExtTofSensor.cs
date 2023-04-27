using BetterTelloLib.Commander.Events.EventArgs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using UnityEngine;

public class ExtTofSensor : MonoBehaviour
{
    private BetterTelloManager tello;
    public float Dist = 10;
    public Obstacle? Prefab;
    private Transform telloTransform;
    public LayerMask ObstacleMask;
    public GameObject Parent;
    private ShowGoldenPath showGolden;

    private bool UpdateRecieved = false;
    public int ExtTof;

    private void Awake()
    {
        showGolden = GetComponent<ShowGoldenPath>();
        tello = GetComponent<BetterTelloManager>();
        telloTransform = GetComponent<Transform>();
    }

    void FixedUpdate()
    {
        
    }

    private void Start()
    {
        tello.BetterTello.Events.OnExtTof += TofRecieved;
    }

    private void Update()
    {
        if (UpdateRecieved && Prefab != null && ExtTof < 4000 && ExtTof >= 200)
        {
            UpdateRecieved = false;
            Vector3 playerPos = telloTransform.position;
            Vector3 playerDirection = telloTransform.forward;
            Quaternion playerRotation = telloTransform.rotation;
            Vector3 spawnPos = playerPos + playerDirection * ExtTof / Dist;
            Obstacle s = Instantiate(Prefab, new Vector3(spawnPos.x, 0, spawnPos.z), playerRotation, Parent.GetComponent<Transform>().transform);
            s.ExtTof = ExtTof;
            s.Transform = telloTransform;
        }

        RaycastHit[] hit = Physics.RaycastAll(transform.position, transform.TransformDirection(Vector3.forward), maxDistance: 0.2f * 100, layerMask: ObstacleMask);
        if (hit.Length > 0)
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit[0].distance, Color.green);
            if (ExtTof >= 4000)
                foreach (var item in hit)
                    Destroy(item.collider.gameObject);
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 0.2f * 100f, Color.red);
        }

    }
    public 

    void OnApplicationQuit()
    {
        tello.BetterTello.Events.OnExtTof -= TofRecieved;
    }

    private void TofRecieved(object sender, ExtTofEventArgs e)
    {
        UpdateRecieved = true;
        ExtTof = e.tof;
    }
}
