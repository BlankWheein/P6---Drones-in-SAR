using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum SearchPattern
{
    Spiral
}
public class SearchPatternBase : MonoBehaviour
{
    public SearchPattern SelectedPattern;
    private Transform m_transform;
    private BetterTelloManager m_manager;
    public void Instantiate()
    {
        if (BetterTelloManager.Targets.Count == 0)
        if (GetPattern() != null)
        {
            Debug.Log("Creating pattern!!!");
            GetPattern().Instantiate(m_transform, (v) => {
                Debug.Log($"Adding target {v}");
                m_manager.AddTarget(v);
            });
            Debug.Log("Done with pattern");
        }
    }

    private void Start()
    {
        
    }
    public ISearchPattern? GetPattern()
    {
        if (SelectedPattern == SearchPattern.Spiral)
            return new SpiralSearchPattern();
        else return null;
    }

    void Awake()
    {
        m_transform = GetComponent<Transform>();
        m_manager = GetComponent<BetterTelloManager>();
    }
}
