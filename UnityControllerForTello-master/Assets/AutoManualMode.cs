using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AutoManualMode : MonoBehaviour
{
    public TMP_Text ControlMode;
    public RawImage MainScreenTexture;
    public RawImage MiniScreenTexture;
    // Start is called before the first frame update
    void Start()
    {
        ControlMode.SetText("Auto");
    }

    // Update is called once per frame
   public void ChangeMode ()
    {
        if (ControlMode.text == "Auto")
        {
            // Change the text of the button + Alignment
            ControlMode.SetText("Manual");
            ControlMode.rectTransform.localPosition = new Vector3(1.5f, 0.5f, 0.0f);
            
        }

        else if (ControlMode.text == "Manual")
        {   
            // Change the text of the button + Alignment
            ControlMode.SetText("Auto");
            ControlMode.rectTransform.localPosition = new Vector3(10.0f, 0.5f, 0.0f);
        }
    }
}
