using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Reflection;

public class StartRunning : MonoBehaviour
{
    public Transform DroneTransform;
    public GameObject LowBatteryWarning;
    public GameObject LowBatteryIcon;
    public TMP_Text dumpText;
    public TMP_Text InfoField;
    public RectTransform InfoFieldBG;
    
    BetterTelloManager Drone;

    private bool expandInputfield = true;
    private bool showInputField = false;
    private bool isWarningDisabled = false;
    private int initialBatteryPercent;

    void Start()
    {
        Drone = GameObject.Find("Drone").GetComponent<BetterTelloManager>();
        DroneTransform = Drone.GetComponent<Transform>();
        //Set the battery percent of the drone when it started 
        initialBatteryPercent = Drone.Bat;

        //Disable the Warning panel from showing
        LowBatteryWarning.SetActive(false);
    }

    void FixedUpdate()
    {
        dumpText.SetText($" Distance: {Drone.DistanceToTarget} \n IsPathfinding: {Drone.IsPathfinding} \n ExtTof: {Drone.ExtTof} \n Pos: {Drone.transform.position} \n Temp: {Drone.BetterTello.State.Templ}/{Drone.BetterTello.State.Temph}\n Path: {Drone.ShowGoldenPath.status}\n Connection :{Drone.ConnectionState}");
        

        if ((Drone.Bat <= initialBatteryPercent / 2 && isWarningDisabled == false || Drone.Bat <= 20 && isWarningDisabled == false) 
            && Drone.Bat != -1 && Drone.ConnectionState == TelloConnectionState.Connected)
            LowBatteryWarning.SetActive(true);
        else 
            LowBatteryWarning.SetActive(false);

        UpdateInfofield();
    }

    public void UpdateInfofield()
    {
        if (Drone.ConnectionState == TelloConnectionState.Connected)
        {
            showInputField=true;
            if (!expandInputfield)
            {
                InfoField.SetText($"Connected to drone \n Battery:{Drone.Bat}\n State: {Drone.FlyingState}\n Coordinates: {Drone.transform.position}\n Target distance: {Drone.DistanceToTarget}");
                InfoFieldBG.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, InfoField.renderedHeight + 25);
            }
            else
            {
                InfoField.SetText($"Connected to drone \n Battery:{Drone.Bat}");
                InfoFieldBG.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, InfoField.renderedHeight + 15);
            }
        }
        else
        {
            showInputField = false;
            expandInputfield = true;
            InfoField.SetText("Not connected to drone");
            InfoField.color = Color.red;
            InfoFieldBG.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, InfoField.renderedHeight + 15);
        }
    }
    public void ExpandInfofield()
    {
        if (showInputField) {
            expandInputfield = !expandInputfield;
                
        }
    }

    public void DisableWarning ()
    {
        isWarningDisabled = true;
        LowBatteryIcon.SetActive(true);
    }
}
