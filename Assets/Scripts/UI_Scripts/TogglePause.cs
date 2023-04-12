using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TogglePause : MonoBehaviour
{
    public TMP_Dropdown SearchPatternDropdown;
    public Image PlayPause;
    public Texture2D Takeoff;
    public Texture2D Land;
    public Texture2D NotConnected;
    BetterTelloManager BetterTelloManager;

    private Sprite _takeoff, _land, _notConnected;

    private void Start()
    {
        BetterTelloManager = GameObject.Find("Drone").GetComponent<BetterTelloManager>();
        _takeoff = Sprite.Create(Takeoff, new Rect(0, 0, Takeoff.width, Takeoff.height), new Vector2());
        _land = Sprite.Create(Land, new Rect(0, 0, Land.width, Land.height), new Vector2());
        _notConnected = Sprite.Create(NotConnected, new Rect(0, 0, NotConnected.width, NotConnected.height), new Vector2());
    }


    private void FixedUpdate()
    {
        if (BetterTelloManager.ConnectionState == TelloConnectionState.Connected)
        {
            if (BetterTelloManager.FlyingState == BetterTelloLib.Commander.FlyingState.Flying)
            {
                PlayPause.sprite = _land;
            }
            else
            {
                PlayPause.sprite = _takeoff;
            }
        }
        else { 
            PlayPause.sprite = _notConnected;
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
