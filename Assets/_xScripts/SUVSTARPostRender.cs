// Copyright 2015 Google Inc. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using UnityEngine;

/// Performs distortion correction on the rendered stereo screen.  This script
/// and CardboardPreRender work together to draw the whole screen in VR Mode.
/// There should be exactly one of each component in any Cardboard scene. It
/// is part of the _CardboardCamera_ prefab, which is included in
/// _CardboardMain_. The Cardboard script will create one at runtime if the
/// scene doesn't already have it, so generally it is not necessary to manually
/// add it unless you wish to edit the Camera component that it controls.
///
/// In the Unity editor, this script also draws the analog of the UI layer on
/// the phone (alignment marker, settings gear, etc).
[RequireComponent(typeof(Camera))]
public class SUVSTARPostRender : MonoBehaviour
{

    // Convenient accessor to the camera component used through this script.
    public Camera cam { get; private set; }

    /// <summary>
    /// How far to shift the barrel distorted image left and right when the phone is landscape
    /// </summary>
    public float distortionCenterOffsetX;
    /// <summary>
    /// How far to shift the barrel distorted image up and down when the phone is landscape
    /// </summary>
    public float distortionCenterOffsetY;
    /// <summary>
    /// Not used
    /// </summary>
    public float rotationOffsetX; // TO-DO: REMOVE
    /// <summary>
    /// Not used
    /// </summary>
    public float rotationOffsetY; // TO-DO: REMOVE

    /// <summary>
    /// True when building to the phone for the left eye; false when building to the phone for the right eye
    /// </summary>
    public bool isLeft;

    /// <summary>
    /// The RenderTexture that holds the reprojected camera feed
    /// </summary>
    public RenderTexture StereoScreen;

    /// <summary>
    /// Camera that will render a second eye if necessary.
    /// </summary>
    public GameObject RightEyeCamera;

    /// <summary>
    /// If running monoscopic pass-through on one device
    /// and want to render two eyes.
    /// </summary>
    public bool TwoEyesSingleDevice;

    // Distortion mesh parameters.

    // Size of one eye's distortion mesh grid.  The whole mesh is two of these grids side by side.
    private const int kMeshWidth = 40;
    private const int kMeshHeight = 40;
    // Whether to apply distortion in the grid coordinates or in the texture coordinates.
    private const bool kDistortVertices = true;

    private Mesh distortionMesh;
    public Material meshMaterial;

    // UI Layer parameters.
    private Material uiMaterial;
    private float centerWidthPx;
    private float buttonWidthPx;
    private float xScale;
    private float yScale;
    private Matrix4x4 xfm;

    // The context panel with a schedule; offset for each eye
    public GameObject TimeQuadL;
    public GameObject TimeQuadR;

    // Text that displays the offset on the UI panel at the configuration zone
    public UnityEngine.UI.Text distXText;
    public UnityEngine.UI.Text distYText;

    // The orthographic camera that reprojects the ARCore feed
    public Transform EyeCamera;

    /// <summary>
    /// Control panel UI that lets you adjust values at runtime.
    /// </summary>
    public GameObject ControlPanelUI;

    void Reset()
    {
#if UNITY_EDITOR
        // Member variable 'cam' not always initialized when this method called in Editor.
        // So, we'll just make a local of the same name.
        var cam = GetComponent<Camera>();
#endif
        cam.clearFlags = CameraClearFlags.SolidColor; //CameraClearFlags.Depth;
        cam.backgroundColor = Color.black;  // Should be noticeable if the clear flags change.
        cam.orthographic = true;
        cam.orthographicSize = 0.5f;
        cam.cullingMask = 0;
        cam.useOcclusionCulling = false;
        cam.depth = 100;
    }

    void Awake()
    {
        cam = GetComponent<Camera>();
        Reset();
        // meshMaterial = new Material(Shader.Find("Cardboard/UnlitTexture"));
        uiMaterial = new Material(Shader.Find("Cardboard/SolidColor"));
        uiMaterial.color = new Color(0.8f, 0.8f, 0.8f);
        // Set the appropriate panel active
        TimeQuadL.SetActive(isLeft);
        TimeQuadR.SetActive(!isLeft);

        // Adjust barrel distortion offsets for the correct eye
        if (isLeft)
        {
            distortionCenterOffsetY *= -1f;
        }
        if (TwoEyesSingleDevice)
        {
            RightEyeCamera.SetActive(true);
            RightEyeCamera.GetComponent<Camera>().enabled = true;
            distortionCenterOffsetX = 0f;
            distortionCenterOffsetY = 0f;
            ControlPanelUI.SetActive(false);
        }
    }

#if UNITY_EDITOR
    private float aspectComparison;

    void OnPreCull()
    {
        // The Game window's aspect ratio may not match the fake device parameters.
        float realAspect = (float)Screen.width / Screen.height;
        float fakeAspect = Screen.width / Screen.height;
        aspectComparison = fakeAspect / realAspect;
        cam.orthographicSize = 0.5f * Mathf.Max(1, aspectComparison);
    }
#endif

    void OnRenderObject()
    {
        if (Camera.current != cam)
        {
            //Debug.Log("not current cam");
            return;

        }
        else
        {
            // Debug.Log("Curcam>: " + Camera.current.name);
        }

        RenderTexture stereoScreen = StereoScreen;

        if (distortionMesh == null)
        { //|| Cardboard.SDK.ProfileChanged) {
            RebuildDistortionMesh();
        }
        meshMaterial.mainTexture = stereoScreen;
        meshMaterial.SetPass(0);
        Graphics.DrawMeshNow(distortionMesh, transform.position + new Vector3(distortionCenterOffsetX, distortionCenterOffsetY, 0f) + new Vector3(rotationOffsetX, rotationOffsetY, 0f), transform.rotation); // * Quaternion.Euler(0f,0f,90f));
        stereoScreen.DiscardContents();

    }

    void UpdateDistortionXText()
    {
        distXText.text = "distX: " + distortionCenterOffsetX.ToString();
        distYText.text = "distY: " + distortionCenterOffsetY.ToString();
    }

    public void MoveLeft()
    {
        distortionCenterOffsetX -= 0.01f;
        UpdateDistortionXText();
    }

    public void MoveRight()
    {
        distortionCenterOffsetX += 0.01f;
        UpdateDistortionXText();

    }

    public void MoveUp()
    {
        distortionCenterOffsetY += 0.01f;
        UpdateDistortionXText();

    }

    public void MoveDown()
    {
        distortionCenterOffsetY -= 0.01f;
        UpdateDistortionXText();

    }

    private void RebuildDistortionMesh()
    {
        distortionMesh = new Mesh();
        Vector3[] vertices;
        Vector2[] tex;
        ComputeMeshPoints(kMeshWidth, kMeshHeight, kDistortVertices, out vertices, out tex, TwoEyesSingleDevice ? 2 : 1);
        //ComputeMeshPoints(kMeshHeight, kMeshWidth, kDistortVertices, out vertices, out tex);
        int[] indices = ComputeMeshIndices(kMeshWidth, kMeshHeight, kDistortVertices);
        //int[] indices = ComputeMeshIndices(kMeshHeight, kMeshWidth, kDistortVertices);
        Color[] colors = ComputeMeshColors(kMeshWidth, kMeshHeight, tex, indices, kDistortVertices);
        // Color[] colors = ComputeMeshColors(kMeshHeight, kMeshWidth, tex, indices, kDistortVertices);
        distortionMesh.vertices = vertices;

        distortionMesh.uv = tex;
        distortionMesh.colors = colors;
        distortionMesh.triangles = indices;
        ;
        distortionMesh.UploadMeshData(true);
    }

    private static void ComputeMeshPoints(int width, int height, bool distortVertices,
                                          out Vector3[] vertices, out Vector2[] tex, int eyeCount = 1)
    {
        float[] lensFrustum = new float[4];
        float[] noLensFrustum = new float[4];
        Rect viewport;
        SUVSTARProfile profile = SUVSTARProfile.Instance;
        profile.GetLeftEyeVisibleTanAngles(lensFrustum);
        profile.GetLeftEyeNoLensTanAngles(noLensFrustum);
        viewport = profile.GetLeftEyeVisibleScreenRect(noLensFrustum);
        vertices = new Vector3[2 * width * height];
        tex = new Vector2[2 * width * height];
        for (int e = 0, vidx = 0; e < eyeCount; e++)
        { // In Cardboard, e < 2, but we only need one eye
            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++, vidx++)
                {
                    float u = (float)i / (width - 1);
                    float v = (float)j / (height - 1);
                    float s, t;  // The texture coordinates in StereoScreen to read from.
                    if (distortVertices)
                    {
                        // Grid points regularly spaced in StreoScreen, and barrel distorted in the mesh.
                        s = u;
                        t = v;
                        float x = Mathf.Lerp(lensFrustum[0], lensFrustum[2], u);
                        float y = Mathf.Lerp(lensFrustum[3], lensFrustum[1], v);
                        float d = Mathf.Sqrt(x * x + y * y);
                        float r = profile.distortInv(d);
                        float p = x * r / d;
                        float q = y * r / d;
                        u = (p - noLensFrustum[0]) / (noLensFrustum[2] - noLensFrustum[0]);
                        v = (q - noLensFrustum[3]) / (noLensFrustum[1] - noLensFrustum[3]);
                    }
                    else
                    {
                        // Grid points regularly spaced in the mesh, and pincushion distorted in
                        // StereoScreen.
                        float p = Mathf.Lerp(noLensFrustum[0], noLensFrustum[2], u);
                        float q = Mathf.Lerp(noLensFrustum[3], noLensFrustum[1], v);
                        float r = Mathf.Sqrt(p * p + q * q);
                        float d = profile.distort(r);
                        float x = p * d / r;
                        float y = q * d / r;
                        s = Mathf.Clamp01((x - lensFrustum[0]) / (lensFrustum[2] - lensFrustum[0]));
                        t = Mathf.Clamp01((y - lensFrustum[3]) / (lensFrustum[1] - lensFrustum[3]));
                    }
                    // Convert u,v to mesh screen coordinates.
                    float aspect = Screen.width / Screen.height;
                    u = (viewport.x + u * viewport.width - 0.5f) * aspect;
                    v = viewport.y + v * viewport.height - 0.5f;
                    vertices[vidx] = new Vector3(u, v, 1);
                    // Adjust s to account for left/right split in StereoScreen.
                    s = (s + e) / 2;
                    tex[vidx] = new Vector2(s, t);
                }
            }
            float w = lensFrustum[2] - lensFrustum[0];
            lensFrustum[0] = -(w + lensFrustum[0]);
            lensFrustum[2] = w - lensFrustum[2];
            w = noLensFrustum[2] - noLensFrustum[0];
            noLensFrustum[0] = -(w + noLensFrustum[0]);
            noLensFrustum[2] = w - noLensFrustum[2];
            viewport.x = 1 - (viewport.x + viewport.width);
        }
    }

    private static Color[] ComputeMeshColors(int width, int height, Vector2[] tex, int[] indices,
                                               bool distortVertices)
    {
        Color[] colors = new Color[2 * width * height];
        for (int e = 0, vidx = 0; e < 2; e++)
        {
            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++, vidx++)
                {
                    colors[vidx] = Color.white;
                    if (distortVertices)
                    {
                        if (i == 0 || j == 0 || i == (width - 1) || j == (height - 1))
                        {
                            colors[vidx] = Color.black;
                        }
                    }
                    else
                    {
                        Vector2 t = tex[vidx];
                        t.x = Mathf.Abs(t.x * 2 - 1);
                        if (t.x <= 0 || t.y <= 0 || t.x >= 1 || t.y >= 1)
                        {
                            colors[vidx] = Color.black;
                        }
                    }
                }
            }
        }
        return colors;
    }

    private static int[] ComputeMeshIndices(int width, int height, bool distortVertices)
    {
        int[] indices = new int[2 * (width - 1) * (height - 1) * 6];
        int halfwidth = width / 2;
        int halfheight = height / 2;
        for (int e = 0, vidx = 0, iidx = 0; e < 2; e++)
        {
            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++, vidx++)
                {
                    if (i == 0 || j == 0)
                        continue;
                    // Build a quad.  Lower right and upper left quadrants have quads with the triangle
                    // diagonal flipped to get the vignette to interpolate correctly.
                    if ((i <= halfwidth) == (j <= halfheight))
                    {
                        // Quad diagonal lower left to upper right.
                        indices[iidx++] = vidx;
                        indices[iidx++] = vidx - width;
                        indices[iidx++] = vidx - width - 1;
                        indices[iidx++] = vidx - width - 1;
                        indices[iidx++] = vidx - 1;
                        indices[iidx++] = vidx;
                    }
                    else
                    {
                        // Quad diagonal upper left to lower right.
                        indices[iidx++] = vidx - 1;
                        indices[iidx++] = vidx;
                        indices[iidx++] = vidx - width;
                        indices[iidx++] = vidx - width;
                        indices[iidx++] = vidx - width - 1;
                        indices[iidx++] = vidx - 1;
                    }
                }
            }
        }
        return indices;
    }
}
