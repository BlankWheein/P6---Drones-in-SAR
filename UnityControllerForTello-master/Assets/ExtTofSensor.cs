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
    public Obstacle? Prefab;
    private Transform telloTransform;
    public GameObject Parent;
    List<Obstacle> Obstacles = new();

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
        if (UpdateRecieved && Prefab != null && ExtTof < 4000)
        {
            UpdateRecieved = false;
            Vector3 playerPos = telloTransform.position;
            Vector3 playerDirection = telloTransform.forward;
            Quaternion playerRotation = telloTransform.rotation;
            Vector3 spawnPos = playerPos + playerDirection * ExtTof / 10;
            Obstacle s = Instantiate(Prefab, new Vector3(spawnPos.x, 0, spawnPos.z), playerRotation, Parent.GetComponent<Transform>().transform);
            s.ExtTof = ExtTof;
            s.Transform = telloTransform;
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
