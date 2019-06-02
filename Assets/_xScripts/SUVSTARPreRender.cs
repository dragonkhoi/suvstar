using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SUVSTARPreRender : MonoBehaviour
{
    public RenderTexture StereoScreen;
    public Camera cam { get; private set; }

    // Start is called before the first frame update
    void Awake()
    {
    cam = GetComponent<Camera>();

    }

    void Reset()
    {
#if UNITY_EDITOR
        // Member variable 'cam' not always initialized when this method called in Editor.
        // So, we'll just make a local of the same name.
        var cam = GetComponent<Camera>();
#endif
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = Color.black;
        cam.cullingMask = 0;
        cam.useOcclusionCulling = false;
        cam.depth = -100;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnPreCull()
    {
        // The Game window's aspect ratio may not match the fake device parameters.
        cam.clearFlags = CameraClearFlags.SolidColor;
        var stereoScreen = StereoScreen;
        if (stereoScreen != null && !stereoScreen.IsCreated())
        {
            stereoScreen.Create();
        }
    }
}
