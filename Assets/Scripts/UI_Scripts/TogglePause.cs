using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditorInternal;

public class TogglePause : MonoBehaviour
{
    public TMP_Dropdown SearchPatternDropdown;
    public Button TakeOff;
    //public Texture2D Takeoff;
  //  public Texture2D Land;
  //  public Texture2D NotConnected;
    BetterTelloManager BetterTelloManager;
    TMP_Text TakeOffText;

   // private Sprite _takeoff, _land, _notConnected;

    private void Start()
    {
        BetterTelloManager = GameObject.Find("Drone").GetComponent<BetterTelloManager>();

        TakeOffText= TakeOff.GetComponentInChildren<TMP_Text>();
    
        /* _takeoff = Sprite.Create(Takeoff, new Rect(0, 0, Takeoff.width, Takeoff.height), new Vector2());
         _land = Sprite.Create(Land, new Rect(0, 0, Land.width, Land.height), new Vector2());
         _notConnected = Sprite.Create(NotConnected, new Rect(0, 0, NotConnected.width, NotConnected.height), new Vector2());
        */
    }


    private void FixedUpdate()
    {
        if (BetterTelloManager.ConnectionState == TelloConnectionState.Connected)
        {
            if (BetterTelloManager.FlyingState == BetterTelloLib.Commander.FlyingState.Flying)
            {
                TakeOffText.text = "Land";
            }
            else
            {
                TakeOffText.text = "TakeOff";
            }
        }
        else { 
           
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
