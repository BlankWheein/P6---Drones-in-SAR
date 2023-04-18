using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallelSearchPattern : MonoBehaviour, ISearchPattern
{
    int X = 20, Y = 20;
    int scale = 10;
    public void Instantiate(Transform trans, Action<Vector3> Initiater)
    {
        int x, y, dx, dy;
        x = y = dx = 0;
        dy = -1;
        int t = Mathf.Max(X, Y);
        int maxI = t * t;
        for (int i = 0; i < maxI; i++)
        {
            if ((x == y) || ((x < 0) && (x == -y)) || ((x > 0) && (x == 1 - y)))
            {
                Initiater(new Vector3(trans.position.x + (x * scale), 0.1f, trans.position.z + (y * scale)));
                t = dx;
                dx = -dy;
                dy = t;
            }
            x += dx;
            y += dy;
        }
        
    }

}
