using BetterTelloLib.Commander.Events.EventArgs;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtTofSensor : MonoBehaviour
{
    private BetterTelloManager tello;
    public Obstacle? Prefab;
    private Transform telloTransform;
    public GameObject Parent;

    private bool UpdateRecieved = false;
    private int ExtTof;

    private void Awake()
    {
        tello = GetComponent<BetterTelloManager>();
        telloTransform = GetComponent<Transform>();
    }


    private void Start()
    {
        tello.BetterTello.Events.OnExtTof += TofRecieved;
    }

    private void Update()
    {
        if (UpdateRecieved && Prefab != null && ExtTof < 8190)
        {
            UpdateRecieved = false;
            Vector3 playerPos = telloTransform.position;
            Vector3 playerDirection = telloTransform.forward;
            Quaternion playerRotation = telloTransform.rotation;
            Vector3 spawnPos = playerPos + playerDirection * ExtTof / 10;
            Obstacle s = Instantiate(Prefab, spawnPos, playerRotation, Parent.GetComponent<Transform>().transform);
            s.ExtTof = ExtTof;
            s.Transform = telloTransform;
            //s.GetComponent<Transform>().parent = Parent.GetComponent<Transform>().transform;
        }
    }

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
