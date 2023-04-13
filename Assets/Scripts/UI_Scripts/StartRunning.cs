using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StartRunning : MonoBehaviour
{
    public TMP_Text batteryVal;
    public Transform DroneTransform;
    public GameObject LowBatteryWarning;
    public GameObject LowBatteryIcon;
    public TMP_Text dumpText;
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
        dumpText.SetText($" Distance: {Drone.DistanceToTarget} \n IsPathfinding: {Drone.IsPathfinding} \n ExtTof: {Drone.ExtTof} \n Pos: {Drone.transform.position} \n Temp: {Drone.BetterTello.State.Templ}/{Drone.BetterTello.State.Temph}\n Path: {Drone.ShowGoldenPath.status}");
        batteryVal.SetText(batteryPercent.ToString());
        if ((batteryPercent <= initialBatteryPercent / 2 && isWarningDisabled == false || batteryPercent <= 20 && isWarningDisabled == false) 
            && batteryPercent != -1 && Drone.ConnectionState == TelloConnectionState.Connected)
            LowBatteryWarning.SetActive(true);
        else 
            LowBatteryWarning.SetActive(false);
    }

    public void DisableWarning ()
    {
        isWarningDisabled = true;
        LowBatteryIcon.SetActive(true);
    }
}
