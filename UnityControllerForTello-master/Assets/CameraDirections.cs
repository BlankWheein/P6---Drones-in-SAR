using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDirections : MonoBehaviour
{

    public Transform drone;
    public Camera DroneCam;

    // Start is called before the first frame update
    void Start()
    {
        

    }

    // Update is called once per frame
    void Update()
    {
       
        RotateCamera();
        //zoom in/out
        if (DroneCam.fieldOfView >= 10 && DroneCam.fieldOfView <= 50)
        {
            DroneCam.fieldOfView += Input.GetAxis("Mouse ScrollWheel") * -10;
        }
        if(DroneCam.fieldOfView < 10 || DroneCam.fieldOfView > 50)
        {
            DroneCam.fieldOfView += Input.GetAxis("Mouse ScrollWheel") * 10;
        }
    }

    void RotateCamera()
    {
        float xAxis;
        float yAxis;

        if (Input.GetMouseButton(1))
        {
            yAxis = Input.GetAxis("Mouse Y") * 10;
            DroneCam.transform.RotateAround(drone.position, Vector3.right, yAxis);

        }
        if (Input.GetMouseButton(0))
        {

            xAxis = Input.GetAxis("Mouse X") * -10;
            DroneCam.transform.RotateAround(drone.position, Vector3.down, xAxis);

        }
    }
}
