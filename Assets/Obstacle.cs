using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public int ExtTof = -1;
    public Transform Transform;

    private void Awake()
    {
    }

    private void Start()
    {
        GetComponent<MeshRenderer>().enabled = false;
    }
}
