using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StartRunning : MonoBehaviour
{
    public TMP_Text batteryVal;
    public TMP_Text distance;
    public TMP_Text xVal;
    public TMP_Text yVal;
    public TMP_Text zVal;
    public Transform DroneTransform;
    public GameObject LowBatteryWarning;
    public GameObject LowBatteryIcon;
    public float batteryPercent = 100;
    BetterTelloManager Drone;

    private bool isWarningDisabled = false;
    private float initialBatteryPercent;

    void Start()
    {
        Drone = GameObject.Find("Drone").GetComponent<BetterTelloManager>();
        DroneTransform = Drone.GetComponent<Transform>();
        //Set the battery percent of the drone when it started 
        initialBatteryPercent = batteryPercent;

        //Disable the Warning panel from showing
        LowBatteryWarning.SetActive(false);
    }

    void FixedUpdate()
    {
        batteryPercent = Drone.Bat;
        batteryVal.SetText(batteryPercent.ToString());
        distance.SetText(Drone.DistanceToTarget.ToString("0.##"));
        if ((batteryPercent <= initialBatteryPercent / 2 && isWarningDisabled == false || batteryPercent <= 20 && isWarningDisabled == false) 
            && batteryPercent != -1 && Drone.ConnectionState == TelloConnectionState.Connected)
            LowBatteryWarning.SetActive(true);
        else 
            LowBatteryWarning.SetActive(false);
        UpdateDroneCoordinates();
    }

    public void DisableWarning ()
    {
        isWarningDisabled = true;
        LowBatteryIcon.SetActive(true);
    }

    private void UpdateDroneCoordinates()
    {
        xVal.SetText((DroneTransform.position.x).ToString("0.##"));
        yVal.SetText((DroneTransform.position.y).ToString("0.##"));
        zVal.SetText((DroneTransform.position.z).ToString("0.##"));
    }
}
