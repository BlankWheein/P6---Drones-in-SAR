using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDirections : MonoBehaviour
{

    public Transform drone;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //transform.LookAt(drone);


        RotateCamera();

        //zoom in/out
        transform.position += new Vector3(0f, Input.GetAxis("Mouse ScrollWheel")* -100, 0f);
    }

    void RotateCamera()
    {

        if (Input.GetMouseButton(1))
        {
            float xAxis = Input.GetAxis("Mouse X")*-10;
            float yAxis = Input.GetAxis("Mouse Y")*10;
            //float zAxis = Input.GetAxis("Mouse Z") * 10;



            transform.RotateAround(drone.position, Vector3.right, yAxis);
            transform.RotateAround(drone.position, Vector3.down, xAxis);
   
           // transform.RotateAround(drone.position, Vector3.forward, yAxis);
        }

    }
}
