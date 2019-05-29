using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;
using System;
using UnityEngine.UI;

public class SUVSTARAcquireBytes : MonoBehaviour
{
    public RenderTexture rt;
    public Image imageCanvas;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        using (var image = Frame.CameraImage.AcquireCameraImageBytes())
        {
            if (!image.IsAvailable)
            {
                return;
            }

            _OnImageAvailable(image.Width, image.Height, image.YRowStride, image.Y, 0, image);
        }
    }

    /// <summary>
    /// Handles a new CPU image.
    /// </summary>
    /// <param name="width">Width of the image, in pixels.</param>
    /// <param name="height">Height of the image, in pixels.</param>
    /// <param name="rowStride">Row stride of the image, in pixels.</param>
    /// <param name="pixelBuffer">Pointer to raw image buffer.</param>
    /// <param name="bufferSize">The size of the image buffer, in bytes.</param>
    private void _OnImageAvailable(
        int width, int height, int rowStride, IntPtr pixelBuffer, int bufferSize, CameraImageBytes image)
    {
        Texture2D newTexture = new Texture2D(width, height, TextureFormat.R8, false, false);
        byte[] s_ImageBuffer = new byte[bufferSize];

        System.Runtime.InteropServices.Marshal.Copy(pixelBuffer, s_ImageBuffer, 0, bufferSize);
        newTexture.LoadRawTextureData(s_ImageBuffer);
        newTexture.Apply();
        imageCanvas.material.SetTexture("_ImageTex", newTexture);
    }
}
