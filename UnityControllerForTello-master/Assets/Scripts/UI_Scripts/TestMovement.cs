using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

public class TestMovement : MonoBehaviour
{


    // Update is called once per frame
    void Update()
    {

        if (Input.GetKey("w"))
        {
            transform.position += transform.forward;
        }


        if (Input.GetKey("d"))
        {
            transform.Rotate(0, 1, 0);
        }

        if (Input.GetKey("a"))
        {
            transform.Rotate(0, -1, 0);
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
