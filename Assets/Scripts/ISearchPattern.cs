using System;
using UnityEngine;

public interface ISearchPattern
{
    void Instantiate(Transform t, Action<Vector3> Spawner);

}
