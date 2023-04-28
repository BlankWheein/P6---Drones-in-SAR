using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Obstacle : MonoBehaviour
{
    public int Radius = 4;
    public int Min = 0;
    public int Max = 1300;
    private int _extTof = -1;
    public int ExtTof { get => _extTof; set {
            _extTof = value;
            OnUpdateExtTof();
        } }

    public float NormalizedExtTof() => (float)(ExtTof - Min) / (float)(Max - Min);
    public Transform Transform;
    private NavMeshObstacle obstacle;

    private void Start()
    {
        GetComponent<MeshRenderer>().enabled = true;
    }
    private void OnUpdateExtTof()
    {
        obstacle = GetComponent<NavMeshObstacle>();
        var normal = NormalizedExtTof();
        var res = Mathf.Clamp(normal * Radius, Radius/2, Radius);
        obstacle.radius = res;
    }
}
