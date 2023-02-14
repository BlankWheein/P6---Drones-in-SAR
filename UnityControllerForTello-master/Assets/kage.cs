using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class kage : MonoBehaviour
{
    public Transform kage2;
    public Object prefab;

    private void Start()
    {
        Instantiate(prefab);
    }

}
