using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshShower : MonoBehaviour
{
    void Start()
    {
        //ShowMesh();
    }

    // Generates the NavMesh shape and assigns it to the MeshFilter component.
    void FixedUpdate()
    {
        // NavMesh.CalculateTriangulation returns a NavMeshTriangulation object.
        NavMeshTriangulation meshData = NavMesh.CalculateTriangulation();

        // Create a new mesh and chuck in the NavMesh's vertex and triangle data to form the mesh.
        Mesh mesh = new Mesh();
        mesh.vertices = meshData.vertices;
        mesh.triangles = meshData.indices;

        // Assigns the newly-created mesh to the MeshFilter on the same GameObject.
        GetComponent<MeshFilter>().mesh = mesh;
    }
}
