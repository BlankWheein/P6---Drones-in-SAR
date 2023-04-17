using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetController : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject target;
    private BetterTelloManager betterTelloManager;
    private void Start()
    {
        betterTelloManager = GetComponent<BetterTelloManager>();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
