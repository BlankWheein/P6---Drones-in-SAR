using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TogglePause : MonoBehaviour
{

    public Image PlayPause;
    public Sprite Play;
    public Sprite Pause;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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
