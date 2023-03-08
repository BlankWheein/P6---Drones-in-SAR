using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MeshScript : MonoBehaviour
{

    public int PlaneResolution = 1;
    Mesh mymesh;
    MeshFilter meshFilter;
    List<Vector3> vertices;
    List<int> triangles;
    void Awake()
    {
        mymesh = new Mesh();
        meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = mymesh;
    }

    void Update()
    {
        PlaneResolution = Mathf.Clamp(PlaneResolution, 1, 50);
        GeneratePlane(PlaneResolution);
        AssignMesh();
    }
    private void GeneratePlane(int resolution)
    {
        vertices = new List<Vector3>();
        var obstacles = FindObjectsOfType<Obstacle>().Where(p => Vector3.Distance(p.GetComponent<Transform>().position, transform.position) < 4f).ToList();
        resolution = obstacles.Count;
        foreach (var item in obstacles)
            vertices.Add(item.Transform.position);
        triangles = new List<int>();
        for (int row = 0; row < resolution; row++)
        {
            for (int col = 0; col < resolution; col++)
            {
                int i = (row * col) + row + col;

                triangles.Add(i);
                triangles.Add(i+(resolution) +1);
                triangles.Add(i+(resolution) +2);

                triangles.Add(i);
                triangles.Add(i + (resolution) + 2);
                triangles.Add(i + 1);
            }
        }
    }
    void AssignMesh()
    {
        mymesh.Clear();
        mymesh.vertices = vertices.ToArray();
        mymesh.triangles = triangles.ToArray();
    }
}
