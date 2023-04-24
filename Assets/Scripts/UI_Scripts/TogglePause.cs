using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditorInternal;

public class TogglePause : MonoBehaviour
{
    public Button TakeOff;
    public Button Return;
    public TMP_Dropdown Pattern;

    BetterTelloManager BetterTelloManager;
    TMP_Text TakeOffText;
    TMP_Text ReturnText;
    TMP_Text PatternText;

    private void Start()
    {
        BetterTelloManager = GameObject.Find("Drone").GetComponent<BetterTelloManager>();

        TakeOffText= TakeOff.GetComponentInChildren<TMP_Text>();
        ReturnText = Return.GetComponentInChildren<TMP_Text>();
        PatternText = Pattern.GetComponentInChildren<TMP_Text>();
    }


    private void FixedUpdate()
    {
        if (BetterTelloManager.ConnectionState == TelloConnectionState.Connected)
        {
            TakeOff.interactable = true;
            TakeOffText.color = Color.black;
            if (BetterTelloManager.FlyingState == BetterTelloLib.Commander.FlyingState.Flying)
            {
                TakeOffText.text = "Land";
                Return.interactable= true;
                ReturnText.color = Color.black;
                Pattern.interactable= true;
                PatternText.color = Color.black;
            }
            else
            {
                TakeOffText.text = "TakeOff";
                Return.interactable = false;
                ReturnText.color = Color.grey;
               Pattern.interactable = false;
                PatternText.color = Color.grey;
            }
        }
        else { 
           TakeOff.interactable=false;
            TakeOffText.color = Color.grey;
            Return.interactable = false;
            ReturnText.color = Color.grey;
            Pattern.interactable=false;
            PatternText.color= Color.grey;
        }
    }


    private async void UpdateState()
    {
        if (BetterTelloManager.ConnectionState == TelloConnectionState.Connected)
        {
            if (BetterTelloManager.FlyingState == BetterTelloLib.Commander.FlyingState.Flying)
            {
                await BetterTelloManager.Land();
            }
            else if (BetterTelloManager.FlyingState == BetterTelloLib.Commander.FlyingState.Grounded)
            {
                await BetterTelloManager.Takeoff();
            } 
        }
        else
        {

            Debug.Log("Not connected to tello drone");
        }
    }

    public void OnClick()
    {
        UpdateState();
    }

}
