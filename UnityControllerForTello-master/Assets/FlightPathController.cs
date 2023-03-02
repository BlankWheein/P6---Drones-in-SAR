using System.Collections;
using System.Collections.Generic;
using UnityControllerForTello;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class FlightPathController : MonoBehaviour
{
    public bool drawFlightPath = true;
    public float Magnitude = 0.05f;
    public List<FlightPoint> flightPoints;
    Transform ground, telloGround, telloModel, flightPointsParent;
    BetterTelloManager telloManager;
    DroneSimulator droneSimulator;
    Transform tf;
    private void Awake()
    {
        flightPointsParent = GameObject.Find("Track Points").transform;

        try {
            this.telloManager = GetComponent<BetterTelloManager>();
            tf = telloManager.GetComponent<Transform>();
        } catch
        {
            try
            {
            this.droneSimulator = GetComponent<DroneSimulator>();
            tf = droneSimulator.GetComponent<Transform>();
            }
            catch
            {
                throw new System.Exception("Could not find drone simulator or BetterTelloManager!!!");
            }
        }
    }
    private void FixedUpdate()
    {
        if (drawFlightPath)
            CreateFlightPoint();
    }
    public void CreateFlightPoint()
    {
        if (!drawFlightPath) return;

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
    //public void TakeOff(TelloManager tm)
    //{
    //    flightPoints = new List<FlightPoint>();
    //    StartCoroutine(ExecuteAfterTakeoff(tm));
    //}
    public void TakeOff(BetterTelloManager tm)
    {
        flightPoints = new List<FlightPoint>();
        StartCoroutine(ExecuteAfterTakeoff(tm));
    }
    public void TakeOff(DroneSimulator ds)
    {
        flightPoints = new List<FlightPoint>();
        drawFlightPath = true;
    }
    //IEnumerator ExecuteAfterTakeoff(TelloManager tm)
    //{
    //    yield return new WaitForSeconds(5);
    //    tm.SetOffset();
    //    drawFlightPath = true;
    //}
    IEnumerator ExecuteAfterTakeoff(BetterTelloManager tm)
    {
        yield return new WaitForSeconds(5);
        drawFlightPath = true;
    }
}
