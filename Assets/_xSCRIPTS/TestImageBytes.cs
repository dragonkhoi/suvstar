using GoogleARCoreInternal;
using System;
using TMPro;
using UnityEngine;

public class TestImageBytes : MonoBehaviour
{
    [SerializeField] Material testMat;
    [SerializeField] TextMeshPro text;

    Texture2D tex;

    void Update()
    {
        //tex = ARCoreAndroidLifecycleManager.Instance.BackgroundTexture;
        tex = Texture2D.CreateExternalTexture(0, 0, TextureFormat.ARGB32, false, false, new IntPtr(27));
        testMat.SetTexture("_MainTex", tex);
        text.text = string.Format("Width: {0} | Height: {1}", tex.width, tex.height);
    }
}
