using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditorInternal;
using System.CodeDom;

public class TogglePause : MonoBehaviour
{
    public Button TakeOff;
    public Button Return;
    public TMP_Dropdown Pattern;
    public Button Clear;

    
    TMP_Text TakeOffText;
    TMP_Text ReturnText;
    TMP_Text PatternText;
    TMP_Text ClearText;

    BetterTelloManager BetterTelloManager;

    Color customColor = new Color(0f, 0f, 0f, 0.3f);

    private void Start()
    {
        BetterTelloManager = GameObject.Find("Drone").GetComponent<BetterTelloManager>();

        TakeOffText= TakeOff.GetComponentInChildren<TMP_Text>();
        ReturnText = Return.GetComponentInChildren<TMP_Text>();
        PatternText = Pattern.GetComponentInChildren<TMP_Text>();
        ClearText = Clear.GetComponentInChildren<TMP_Text>();

    }


    private void FixedUpdate()
    {

        updateButtons();
     
    }

    private void updateButtons()
    {
        if (BetterTelloManager.ConnectionState == TelloConnectionState.Connected)
        {
            TakeOff.interactable = true;
            TakeOffText.color = Color.black;
            if (BetterTelloManager.FlyingState == BetterTelloLib.Commander.FlyingState.Flying)
            {
                TakeOffText.text = "Land";
                Return.interactable = true;
                ReturnText.color = Color.black;
                Pattern.interactable = true;
                PatternText.color = Color.black;
            }
            else
            {
                TakeOffText.text = "TakeOff";
                Return.interactable = false;
                ReturnText.color = customColor;
            }
        }
        else
        {
            TakeOff.interactable = false;
            TakeOffText.color = customColor;
            Return.interactable = false;
            ReturnText.color = customColor;
            Pattern.interactable = false;
            PatternText.color = customColor;
        }

        // if there are no targets(i.e. no path), you can't reset path
        if (BetterTelloManager.Targets.Count == 0)
        {
            Pattern.interactable = false;
            PatternText.color = customColor;
        }
        else
        {
            Pattern.interactable = true;
            PatternText.color = Color.black;
        }
    }


    private async void UpdateFlightState()
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
        UpdateFlightState();
    }

    public void OnClearClick()
    {
        //call the clear target function
    }

}
