using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TogglePause : MonoBehaviour
{
    public TMP_Dropdown SearchPatternDropdown;
    public Image PlayPause;
    public Sprite Takeoff;
    public Sprite Land;
    

    private void ChangeImage()
    {
        if(PlayPause.sprite == Takeoff)
        {
            PlayPause.sprite = Land;
        }
        else
        {
            PlayPause.sprite = Takeoff;
        }
    }

    private void ChangeInteractibility () {

        
        if(PlayPause.sprite == Land) 
        {
            SearchPatternDropdown.interactable = true;
        }
        else if (PlayPause.sprite == Takeoff) 
        { 
            SearchPatternDropdown.interactable = false; 
        }
    }

    public void OnClick()
    {
        ChangeImage();
        ChangeInteractibility();
    }

}
