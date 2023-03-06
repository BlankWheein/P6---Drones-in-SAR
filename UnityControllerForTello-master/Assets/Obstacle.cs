using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public int ExtTof = -1;
    public Transform Transform;

    private float distance = 4f;
    private List<Obstacle> obstacles;

    private void Awake()
    {
        Transform = GetComponent<Transform>();
    }
}
