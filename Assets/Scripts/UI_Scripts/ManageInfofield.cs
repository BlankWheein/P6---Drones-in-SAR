using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Reflection;
using System;

public class ManageInfofield : MonoBehaviour
{
    public Transform DroneTransform;
    public GameObject LowBatteryWarning;
    public TMP_Text dumpText;
    public TMP_Text InfoField;
    public RectTransform InfoFieldBG;
    public GameObject expandBtn;

    BetterTelloManager Drone;
 
    private bool IsInfofieldExpanded = false;
    private bool showFullInfoField = false;
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
        if (initialBatteryPercent == 0)
        {
            initialBatteryPercent = Drone.Bat;
        }
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
            showFullInfoField=true;
            InfoField.color = Color.black;
            expandBtn.SetActive(true);
            if (IsInfofieldExpanded)
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
            showFullInfoField = false;
            IsInfofieldExpanded = false;
            expandBtn.SetActive(false);
            InfoField.color = Color.red;

            InfoField.SetText("Not connected to drone");
            InfoFieldBG.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, InfoField.renderedHeight + 15);
            
        }
    }
    public void ExpandInfofield()
    {
        if (showFullInfoField) {
            IsInfofieldExpanded = !IsInfofieldExpanded;
            Console.WriteLine("jfioejoiefjeijeoefo");
            expandBtn.transform.Rotate(Vector3.forward * 180, Space.Self);
        }
    }

    public void DisableWarning ()
    {
        isWarningDisabled = true;
        //LowBatteryIcon.SetActive(true);
    }
}
