using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public int ExtTof = -1;
    public Transform Transform;

    private float distance = 4f;


    private void Awake()
    {
        Transform = GetComponent<Transform>();
        transform.localScale = new Vector3(0, 0, 0);
    }

    void FixedUpdate()
    {
        List<Obstacle> obstacles = FindObjectsOfType<Obstacle>().Where(p => Vector3.Distance(p.GetComponent<Transform>().position, transform.position) < distance).ToList();
        foreach (var item in obstacles)
        {
            Debug.DrawLine(transform.position, item.transform.position, Color.green);
        }
    }

}
