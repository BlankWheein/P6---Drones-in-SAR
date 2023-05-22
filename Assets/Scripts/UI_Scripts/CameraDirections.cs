using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraDirections : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public Transform drone;
    public Camera DroneCam;
    public GameObject mainScreen;
    
    private Transform droneInit;

    private bool isMouseOverScreen=false;

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
        if (isMouseOverScreen)
        {
            //RotateX();
            //zoom in/out
            if (DroneCam.orthographicSize >= 5 && DroneCam.orthographicSize <= 500)
            {
                DroneCam.orthographicSize += Input.GetAxis("Mouse ScrollWheel") * -100;
            }
            if (DroneCam.orthographicSize < 5 || DroneCam.orthographicSize > 500)
            {
                DroneCam.orthographicSize += Input.GetAxis("Mouse ScrollWheel") * 100;
            }
        }
    }

    void RotateX()
    {
        float xAxis;
        if (Input.GetMouseButton(0))
        {
            xAxis = Input.GetAxis("Mouse X") * 10;
            DroneCam.transform.RotateAround(drone.position, Vector3.up, xAxis);
        }
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
       
        isMouseOverScreen = true;

    }

    public void OnPointerExit(PointerEventData eventData)
    {
       
        isMouseOverScreen = false;
    }
    }
