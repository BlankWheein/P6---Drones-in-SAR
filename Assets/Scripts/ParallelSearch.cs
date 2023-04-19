using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallelSearchPattern : MonoBehaviour, ISearchPattern
{
    int Points = 10;
    int scale = 20;
    int zA = 1; int xA = 5;
    public void Instantiate(Transform trans, Action<Vector3> Initiater)
    {
        int x = 0; int z = 0;
        bool forward = false;
        bool backward = false;
        bool first = true;
        scale /= 2;
        for (int i = 0; i < Points; i++)
        {
            
            if (forward)
            {
                forward = false;
                if (backward)
                {
                    x -= xA * scale;
                    backward = false;
                }
                else
                {
                    x += xA * scale;
                    backward = true;
                }
                if (first)
                {
                    scale *= 2;
                    first = false;
                }
            } else
            {
                forward = true;
                z += zA * scale;
            }
            
            Initiater(new Vector3(trans.position.x + x, 0.1f, trans.position.z + z));
        }
    }

}
