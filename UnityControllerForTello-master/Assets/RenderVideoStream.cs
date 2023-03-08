using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RenderVideoStream : MonoBehaviour
{
    public byte[] data;

    private void Update()
    {
        if (data.Length == 0) return;
        Texture2D tex = new Texture2D(1280, 720, TextureFormat.RGBA32, false);
        tex.LoadImage(data);
        tex.filterMode = FilterMode.Point;
        tex.Apply();
        GetComponent<Renderer>().material.mainTexture = tex;
    }
    public void UpdateStream(byte[] stream)
    {
        data = stream;
    }
}
