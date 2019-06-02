using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;

public class SUVSTARCameraFeed : MonoBehaviour
{
    public RenderTexture toRender;
    Renderer rend;
    Material BackgroundMaterial;
    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<Renderer>();
        BackgroundMaterial = rend.material;
    }

    // Update is called once per frame
    void Update()
    {
        //toRender = (RenderTexture)Frame.CameraImage.Texture;
        Graphics.Blit(Frame.CameraImage.Texture, toRender);
        const string mainTexVar = "_MainTex";
        const string topLeftRightVar = "_UvTopLeftRight";
        const string bottomLeftRightVar = "_UvBottomLeftRight";

        BackgroundMaterial.SetTexture(mainTexVar, Frame.CameraImage.Texture);

        var uvQuad = Frame.CameraImage.TextureDisplayUvs;
        BackgroundMaterial.SetVector(
            topLeftRightVar,
            new Vector4(
                uvQuad.TopLeft.x, uvQuad.TopLeft.y, uvQuad.TopRight.x, uvQuad.TopRight.y));
        BackgroundMaterial.SetVector(
            bottomLeftRightVar,
            new Vector4(uvQuad.BottomLeft.x, uvQuad.BottomLeft.y, uvQuad.BottomRight.x,
                uvQuad.BottomRight.y));

        //m_Camera.projectionMatrix = Frame.CameraImage.GetCameraProjectionMatrix(
        //        m_Camera.nearClipPlane, m_Camera.farClipPlane);
    }
}
