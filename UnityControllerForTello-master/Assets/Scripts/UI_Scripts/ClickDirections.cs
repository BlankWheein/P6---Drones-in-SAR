using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class ClickDirections : MonoBehaviour, IPointerClickHandler
{

    [SerializeField] private RectTransform screen;
    [SerializeField] private Camera droneCam;
    [SerializeField] private GameObject directionObject = null;
    [SerializeField] private GameObject drone;
    private GameObject oldBlock = null;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Middle)
        {
            if (oldBlock != null)
            {
                Destroy(oldBlock);
            }

            RectTransformUtility.ScreenPointToLocalPointInRectangle(screen, eventData.position, null, out Vector2 localPoint);

            var rect = screen.rect;
            localPoint.x = (localPoint.x / rect.width) + screen.pivot.x;
            localPoint.y = (localPoint.y / rect.height) + screen.pivot.y;
            Ray ray = droneCam.GetComponent<Camera>().ViewportPointToRay(localPoint);
            Plane plane = new Plane(Vector2.down, Vector2.left);

            plane.Raycast(ray, out float d);
            Vector3 hit = ray.GetPoint(d);


            GameObject clickPoint = Instantiate(directionObject);
            clickPoint.transform.position = new Vector3(hit.x, drone.transform.position.y, hit.z);
            clickPoint.name = "Waypoint";
            oldBlock = clickPoint;

        }

    }
}