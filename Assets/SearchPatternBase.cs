using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchPatternBase : MonoBehaviour
{
    public ESearchPattern SelectedPattern;
    public enum ESearchPattern
    {
        ParallelSearch,
        SpiralSearch
    }


    public void InstantiatePattern()
    {
        Transform t = BetterTelloManager.Targets.Count == 0 ? transform : BetterTelloManager.Targets[^1].GetComponent<Transform>();
        switch (SelectedPattern)
        {
            case ESearchPattern.ParallelSearch:
                ParallelSearch(t);
                break;
            case ESearchPattern.SpiralSearch:
                SpiralSearch(t);
                break;
        }
    }

    private void ParallelSearch(Transform t)
    {
        int Points = 10;
        int scale = 20;
        int zA = 1; int xA = 5;
        int x = 0; int z = 0;
        bool forward = false;
        bool backward = false;
        bool first = true;
        scale /= 2;
        for (int i = 0; i < Points; i++)
        {
            if (i == Points - 1)
                scale /= 2;
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
            }
            else
            {
                forward = true;
                z += zA * scale;
            }
            GetComponent<BetterTelloManager>().AddTarget(new Vector3(t.position.x + x, 0.1f, t.position.z + z));
        }
    }

    private void SpiralSearch(Transform tra)
    {
        int X = 6, Y = 6;
        int scale = 30;
        int x, y, dx, dy;
        x = y = dx = 0;
        dy = -1;
        int t = Mathf.Max(X, Y);
        int maxI = t * t;
        for (int i = 0; i < maxI; i++)
        {
            if ((x == y) || ((x < 0) && (x == -y)) || ((x > 0) && (x == 1 - y)))
            {
                GetComponent<BetterTelloManager>().AddTarget(new Vector3(tra.position.x + (x * scale), 0.1f, tra.position.z + (y * scale)));
                t = dx;
                dx = -dy;
                dy = t;
            }
            x += dx;
            y += dy;
        }
    }
}
