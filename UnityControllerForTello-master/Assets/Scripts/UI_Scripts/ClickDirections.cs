using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class ClickDirections : MonoBehaviour, IPointerClickHandler
    {

    [SerializeField] protected Camera UICamera;
    [SerializeField] protected RectTransform textureRectTransform;
    [SerializeField] protected Camera RenderToTextureCamera;
    [SerializeField] private GameObject directionObject = null;
    private GameObject oldBlock = null;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Middle)
        {
            if(oldBlock != null)
            {
                Destroy(oldBlock);
            }
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(textureRectTransform, eventData.position, UICamera, out localPoint);
            var rect = textureRectTransform.rect;
            localPoint.x = (localPoint.x / rect.width) + textureRectTransform.pivot.x;
            localPoint.y = (localPoint.y / rect.height) + textureRectTransform.pivot.y;
            Ray ray = RenderToTextureCamera.GetComponent<Camera>().ViewportPointToRay(localPoint);
            Plane plane = new Plane(Vector3.up, Vector3.zero);
           
            plane.Raycast(ray, out float d);
            Vector3 hit = ray.GetPoint(d);
            GameObject clickPoint = Instantiate(directionObject);
            clickPoint.transform.position = new Vector3(hit.x, 0, hit.z);
            clickPoint.name = "Waypoint";
            oldBlock =clickPoint;
            Debug.Log("H: " + hit);
        }

    }
}
