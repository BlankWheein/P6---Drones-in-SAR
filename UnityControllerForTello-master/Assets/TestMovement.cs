using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMovement : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown("w"))
        {
            transform.position += new Vector3(0, 0, 10);
        }


        if (Input.GetKeyDown("d"))
        {
            transform.position += new Vector3(10, 0, 0);
        }

        if (Input.GetKeyDown("a"))
        {
            transform.position += new Vector3(-10, 0, 0);
        }
    }
}
