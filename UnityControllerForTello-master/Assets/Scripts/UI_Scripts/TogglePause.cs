using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TogglePause : MonoBehaviour
{

    public Image PlayPause;
    public Sprite Play;
    public Sprite Pause;

    public void ChangeImage()
    {
        if(PlayPause.sprite == Play)
        {
            PlayPause.sprite = Pause;
        }
        else
        {
            PlayPause.sprite = Play;
        }
    }
}
