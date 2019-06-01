using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;

public class SUVSTARCameraFeed : MonoBehaviour
{
    public RenderTexture toRender;
    Renderer rend;
    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        //toRender = (RenderTexture)Frame.CameraImage.Texture;
        Graphics.Blit(Frame.CameraImage.Texture, toRender);
        rend.material.mainTexture = Frame.CameraImage.Texture;

    }
}
