using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;

public class SUVSTARCameraFeed : MonoBehaviour
{
    public RenderTexture toRender;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //toRender = (RenderTexture)Frame.CameraImage.Texture;
        Graphics.Blit(Frame.CameraImage.Texture, toRender);
    }
}
