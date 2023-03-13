using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickDirections : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Canvas parentCanvas = null;        // the parent canvas of this UI - only needed to determine if we need the camera  
    [SerializeField] private RectTransform rect = null;         // the recttransform of the UI object
    [SerializeField] private GameObject directionObject = null;
    [SerializeField] private GameObject drone;
    [SerializeField] private Camera droneCam;
    // you can serialize this as well - do NOT assign it if the canvas render mode is overlay though
    private Camera UICamera = null;                             // the camera that is rendering this UI
    private GameObject oldBlock=null;
    private void Start()
    {
        if (rect == null)
            rect = GetComponent<RectTransform>();

        if (parentCanvas == null)
            parentCanvas = GetComponentInParent<Canvas>();

        if (UICamera == null && parentCanvas.renderMode == RenderMode.ScreenSpaceCamera)
            UICamera = parentCanvas.worldCamera;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        GameObject block;
        Vector3 dronePos = drone.transform.position;
        if (eventData.button == PointerEventData.InputButton.Middle)
        {

            if(oldBlock!= null)
            {
                Destroy(oldBlock);
            }
            // this UI element has been clicked by the mouse so determine the local position on your UI element
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, eventData.position, UICamera, out Vector2 localPos);

            //make it center alligned
            localPos.x += rect.rect.width / 2f;
            localPos.y -= rect.rect.height / 2f;

            //set the position of the wayponint block, to fit with where you click
            directionObject.transform.position = new Vector3(dronePos.x + (localPos.x * droneCam.fieldOfView / 30.1f),
                dronePos.y,
                dronePos.z + (localPos.y * droneCam.fieldOfView / 30.1f));


            block = Instantiate(directionObject);
            block.transform.localScale = new Vector3(20, 20, 20);
            block.name = "Waypoint";

            oldBlock = block;
        }
    }
}