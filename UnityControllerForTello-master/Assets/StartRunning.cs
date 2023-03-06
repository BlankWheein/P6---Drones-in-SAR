using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StartRunning : MonoBehaviour
{
    public TMP_InputField field;
    public TMP_Text distVal;
    public TMP_Text xVal;
    public TMP_Text yVal;
    public TMP_Text zVal;
    public Transform drone;
    private bool onPlay = false;
    private float xVal0;
    private float yVal0;
    private float zVal0;
    // Start is called before the first frame update
    void Start()
    {
        //set the start coordinates of the drone
        xVal0 = drone.transform.position.x;
        yVal0 = drone.transform.position.y;
        zVal0 = drone.transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateDroneCoordinates();
    }

    //called when play button is clicked
    public void OnPlay()
    {   
        InsertDist();
        onPlay= true;
    }

    //insert the input distance in the info panel
    private void InsertDist()
    {
         distVal.text = field.text==""?"0":field.text;
    }

    private void UpdateDroneCoordinates()
    {   
        if (onPlay)
        {     
            //update the infofield witht the coordinates, starting form (0,0,0)
            xVal.SetText((drone.transform.position.x-xVal0).ToString());
            yVal.SetText((drone.transform.position.y-yVal0).ToString());
            zVal.SetText((drone.transform.position.z-zVal0).ToString());
        }
    }
}
