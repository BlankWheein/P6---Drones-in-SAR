using System.Collections;
using System.Collections.Generic;
using UnityControllerForTello;
using System.Collections.Generic;
using UnityEngine;
using TelloLib;
using System.Collections;

public class FlightPathController : MonoBehaviour
{
    bool drawFlightPath = false;
    public float Magnitude = 0.05f;
    public List<FlightPoint> flightPoints;
    public SceneManager sm;
    Transform ground, telloGround, telloModel, flightPointsParent;
    TelloManager telloManager;
    DroneSimulator droneSimulator;
    Transform tf;
    private void Awake()
    {
        flightPointsParent = GameObject.Find("Track Points").transform;

        if (sm.sceneType == SceneManager.SceneType.FlyOnly)
        {
            this.telloManager = GetComponent<TelloManager>();
            tf = telloManager.GetComponent<Transform>();
        }
        else
        {
            this.droneSimulator = GetComponent<DroneSimulator>();
            tf = droneSimulator.GetComponent<Transform>();
        }
    }
    private void FixedUpdate()
    {
        if (sm.flightStatus == SceneManager.FlightStatus.Flying)
            CreateFlightPoint();
    }
    public void CreateFlightPoint()
    {
        if (!drawFlightPath) return;

        if ( sm.sceneType == SceneManager.SceneType.FlyOnly 
            && Mathf.Abs(telloManager.posX) < 0.2 && Mathf.Abs(telloManager.posY) < 0.2 && Mathf.Abs(telloManager.posZ) < 0.2) return;
        
        if (flightPoints.Count > 0)
        {
            Vector3 flightPointDif = flightPoints[flightPoints.Count - 1].transform.position - tf.position;
            if (flightPoints.Count > 0 && !(flightPointDif.magnitude > Magnitude)) return;
        }
            
        Debug.Log("Instatiating point");
        var newPoint = Instantiate(GameObject.Find("FlightPoint")).GetComponent<FlightPoint>();
        newPoint.transform.position = tf.position;
        newPoint.transform.SetParent(flightPointsParent);
        newPoint.CustomStart();

        if (flightPoints.Count > 0)
            newPoint.SetPointOne(flightPoints[flightPoints.Count - 1].transform.position);
        flightPoints.Add(newPoint);
    }
    public void Land()
    {
        drawFlightPath = false;
    }
    public void TakeOff(TelloManager tm)
    {
        flightPoints = new List<FlightPoint>();
        StartCoroutine(ExecuteAfterTakeoff(tm));
    }
    public void TakeOff(DroneSimulator ds)
    {
        flightPoints = new List<FlightPoint>();
        drawFlightPath = true;
    }
    IEnumerator ExecuteAfterTakeoff(TelloManager tm)
    {
        yield return new WaitForSeconds(5);
        tm.SetOffset();
        drawFlightPath = true;
    }
}
