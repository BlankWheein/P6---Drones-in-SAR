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
        }else if (DroneCam.fieldOfView <= 10)
        {
            DroneCam.fieldOfView = 10;
        }
        else
        {
            DroneCam.fieldOfView = 50;
        }
        
       
    }

    void RotateCamera()
    {
        float xAxis;
        float yAxis;

        if (Input.GetMouseButton(1))
        {


            yAxis = Input.GetAxis("Mouse Y") * 10;
            transform.RotateAround(drone.position, Vector3.right, yAxis);


            //does not work properly
           /* if (rotValY >= -90 && rotValY <= 0)
            {
                // xAxis = Input.GetAxis("Mouse X") * -10;
                //  transform.RotateAround(drone.position, Vector3.down, xAxis);

                yAxis = Input.GetAxis("Mouse Y") * 10;
                rotValY += yAxis;
                transform.RotateAround(drone.position, Vector3.right, yAxis);

            }
            else if (rotValY < -90)
            {

                yAxis = Mathf.Abs(Input.GetAxis("Mouse Y") * 10);
                rotValY += yAxis;
                transform.RotateAround(drone.position, Vector3.right, yAxis);
            }
            else
            {
                yAxis = Mathf.Abs(Input.GetAxis("Mouse Y") * 10);
                rotValY -= yAxis;
                transform.RotateAround(drone.position, Vector3.right, -yAxis);
            }*/


        }
        if (Input.GetMouseButton(0))
        {

            xAxis = Input.GetAxis("Mouse X") * -10;
            transform.RotateAround(drone.position, Vector3.down, xAxis);



        }
    }
}
