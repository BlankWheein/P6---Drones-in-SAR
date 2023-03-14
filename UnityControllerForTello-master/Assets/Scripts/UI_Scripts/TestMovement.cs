using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMovement : MonoBehaviour
{


    // Update is called once per frame
    void Update()
    {

        if (Input.GetKey("w"))
        {
            transform.position += new Vector3(0, 0, 1);
        }


        if (Input.GetKey("d"))
        {
            transform.position += new Vector3(1, 0, 0);
        }

        if (Input.GetKey("a"))
        {
            transform.position += new Vector3(-1, 0, 0);
        }


        if (Input.GetKey("up"))
        {
            transform.position += new Vector3(0, 1, 0);
        }
        if (Input.GetKey("down"))
        {
            transform.position += new Vector3(0, -1, 0);
        }
    }
}
