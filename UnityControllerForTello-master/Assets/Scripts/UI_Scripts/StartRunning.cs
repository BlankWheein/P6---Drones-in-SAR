using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StartRunning : MonoBehaviour
{
    public TMP_Text batteryVal;
    public TMP_Text xVal;
    public TMP_Text yVal;
    public TMP_Text zVal;
    public Transform drone;
    public GameObject LowBatteryWarning;
    public float batteryPercent = 100;

    private bool isWarningDisabled = false;
    private bool onPlay = false;
    private float xValInitial;
    private float yValInitial;
    private float zValInitial;
  
    private float initialBatteryPercent;

    // Start is called before the first frame update
    void Start()
    {
        //Set the battery percent of the drone when it started 
        initialBatteryPercent = batteryPercent;

        //Disable the Warning panel from showing
        LowBatteryWarning.SetActive(false);

        //set the start coordinates of the drone
        xValInitial = drone.transform.position.x;
        yValInitial = drone.transform.position.y;
        zValInitial = drone.transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        batteryVal.SetText(batteryPercent.ToString());

        if (batteryPercent <= initialBatteryPercent / 2 && isWarningDisabled == false || batteryPercent <= 20 && isWarningDisabled == false)
        {
            LowBatteryWarning.SetActive(true);
        }
        else { LowBatteryWarning.SetActive(false); }

        if (onPlay)
        {
            UpdateDroneCoordinates();
        }
    }

    public void DisableWarning ()
    {
        isWarningDisabled = true;
    }


    //called when play button is clicked
    public void OnPlay()
    {
        onPlay = true;
    }

    private void UpdateDroneCoordinates()
    {
        //update the infofield witht the coordinates, starting form (0,0,0)
        float xValNew = drone.transform.position.x - xValInitial;
        float yValNew = drone.transform.position.y - yValInitial;
        float zValNew = drone.transform.position.z - zValInitial;

        
        xVal.SetText((xValNew).ToString("0.##"));
        yVal.SetText((yValNew).ToString("0.##"));
        zVal.SetText((zValNew).ToString("0.##"));

       
    }
}
