using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditorInternal;
using System.CodeDom;
using System;
using static SearchPatternBase;
using System.Linq;

public class ManageButtons : MonoBehaviour
{
    public Button TakeOff;
    public Button Return;
    public TMP_Dropdown Pattern;
    public Button Clear;
    public GameObject DroneFans;


    List<Animator> fans = new List<Animator>();


    TMP_Text TakeOffText;
    TMP_Text ReturnText;
    TMP_Text PatternText;
    TMP_Text ClearText;

    BetterTelloManager BetterTelloManager;

    Color customColor = new Color(0f, 0f, 0f, 0.3f);

    private void Start()
    {
        BetterTelloManager = GameObject.Find("Drone").GetComponent<BetterTelloManager>();

        TakeOffText = TakeOff.GetComponentInChildren<TMP_Text>();
        ReturnText = Return.GetComponentInChildren<TMP_Text>();
        PatternText = Pattern.GetComponentInChildren<TMP_Text>();
        ClearText = Clear.GetComponentInChildren<TMP_Text>();

        foreach (Animator a in DroneFans.GetComponentsInChildren<Animator>())
        {
            fans.Add(a);
            a.enabled=false;
        }
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
                ShouldAnimationRun(true);

            }
            else
            {
                TakeOffText.text = "TakeOff";
                Return.interactable = false;
                ReturnText.color = customColor;
               ShouldAnimationRun(false);
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
            ShouldAnimationRun(false);
        }

        // if there are no targets(i.e. no path), you can't reset path
        if (BetterTelloManager.Targets.Count == 0)
        {
            Clear.interactable = false;
            ClearText.color = customColor;  
        }
        else
        {
            Clear.interactable = true;
            ClearText.color = Color.black;
        }
    }

    private void ShouldAnimationRun (bool shouldRun){
        if (shouldRun)
        {
            foreach(Animator  a in fans)
            {
                a.enabled = true;
            }
        }
        else
        {
            foreach (Animator a in fans)
            {
                a.enabled = false;
            }
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

    public void OnTakeOffClick()
    {
        UpdateFlightState();
    }

    public void OnClearClick()
    {
        Debug.Log("Clearing targets");
        BetterTelloManager.RemoveAllTargets();
    }

    public void OnReturnClick() {
        BetterTelloManager.ReturnHome();
    }

    public void OnSearchPatternChanged()
    {
        string PatternName="";
        if (PatternText.text=="Parallel Track")
        {
            PatternName = "SpiralSearch";
        }
        else if( PatternText.text=="Expanding Square")
        {
            PatternName = "ParallelSearch";
        }
     

        if (Enum.TryParse(PatternName, out ESearchPattern res))
        {
            BetterTelloManager.GetComponent<SearchPatternBase>().SelectedPattern = res;
            BetterTelloManager.GetComponent<SearchPatternBase>().InstantiatePattern();
            Pattern.value = 0;
        }
    }

}
