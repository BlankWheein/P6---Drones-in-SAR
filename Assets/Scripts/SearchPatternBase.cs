using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum SearchPattern
{
    Spiral = 1 << 0,
    Parallel = 1 << 1,
}
public class SearchPatternBase : MonoBehaviour
{
    public SearchPattern SelectedPattern;
    private Transform m_transform;
    private BetterTelloManager m_manager;
    public void Instantiate()
    {
        if (BetterTelloManager.Targets.Count == 0)
        Pattern?.Instantiate(m_transform, (v) => {
                m_manager.AddTarget(v);
            });
    }
    #nullable enable
    public ISearchPattern? Pattern
    {
        get
        {
            if (SelectedPattern == SearchPattern.Spiral)
                return new SpiralSearchPattern();
            else if (SelectedPattern == SearchPattern.Parallel)
                return new ParallelSearchPattern();
            else return null;
        }
    }
    #nullable disable

    void Awake()
    {
        m_transform = GetComponent<Transform>();
        m_manager = GetComponent<BetterTelloManager>();
        SelectedPattern = SearchPattern.Spiral;
    }
}
