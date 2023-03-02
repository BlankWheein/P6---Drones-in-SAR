using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public int ExtTof = -1;
    public Transform Transform;


    private void Awake()
    {
        Transform = GetComponent<Transform>();
    }
}
