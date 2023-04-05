using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetTexture : MonoBehaviour
{
    [SerializeField]
    public GameObject TargetTexture;
    public RenderTexture droneview;
    public Camera camera;
    // Start is called before the first frame update
    void Start()
    {
        camera.targetTexture = droneview;
    }

    // Update is called once per frame
    void Update()
    {
        //TargetTexture.GetComponent<RawImage>().texture = RTImage(camera);
    }

    Texture2D RTImage(Camera camera)
    {
        // The Render Texture in RenderTexture.active is the one
        // that will be read by ReadPixels.
        var currentRT = RenderTexture.active;
        RenderTexture.active = camera.targetTexture;

        // Render the camera's view.
        camera.Render();

        // Make a new texture and read the active Render Texture into it.
        Texture2D image = new Texture2D(camera.targetTexture.width, camera.targetTexture.height);
        image.ReadPixels(new Rect(0, 0, camera.targetTexture.width, camera.targetTexture.height), 0, 0);
        image.Apply();

        // Replace the original active Render Texture.
        RenderTexture.active = currentRT;
        return image;
    }
}
