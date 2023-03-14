using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Profiling;
using UnityEngine;

public class CameraDirections : MonoBehaviour
{

    public Transform drone;
    public Camera DroneCam;
    public TMP_Text modeName;
    private Transform droneInit;
    private bool is3D=false;

    // Start is called before the first frame update
    void Start()
    {
        //save the start values of the camera, for use when toggling to 2D mode
        droneInit = new GameObject().transform;
        float posX=DroneCam.transform.localPosition.x;
        float posY=DroneCam.transform.localPosition.y;
        float posZ=DroneCam.transform.localPosition.z;

        //the rotation values are just from the inspector
        Quaternion rotationInit =Quaternion.Euler(90, 0, 0);
        droneInit.rotation = rotationInit;

        droneInit.position=new Vector3(posX,posY, posZ);

    }

    // Update is called once per frame
    void Update()
    {
       if(is3D) {
            RotateCamera();
        }
        
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

    public void toggle3D()
    {
        is3D= !is3D;
        if (is3D == false)
        {
            DroneCam.transform.SetLocalPositionAndRotation(droneInit.position,droneInit.rotation);
            modeName.text = "2D";
        }
        else
        {
            modeName.text = "3D";
        }
    }
}
