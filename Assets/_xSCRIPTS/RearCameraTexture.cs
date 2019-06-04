using TMPro;
using UnityEngine;
using UnityEngine.Android;

public class RearCameraTexture : MonoBehaviour
{
    //[SerializeField] RawImage background;
    [SerializeField] Renderer screenRenderer;
    //[SerializeField] AspectRatioFitter fitter;
    [SerializeField] TextMeshPro debugText;

    bool cameraAvailable;
    WebCamTexture backCamera;
    //Texture defaultBackground;

    void Start()
    {
#if PLATFORM_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            Permission.RequestUserPermission(Permission.Camera);
        }
#endif

        // TODO: The below doesn't seem to run if permissions were not granted yet.
        
        //defaultBackground = background.texture;
        WebCamDevice[] devices = WebCamTexture.devices;

        if (devices.Length == 0)
        {
            Debug.LogError("No camera detected");
            cameraAvailable = false;
            return;
        }

#if UNITY_EDITOR
        if (devices.Length > 0)
        {
            backCamera = new WebCamTexture(devices[0].name, 1280, 1280);
        }
#else
        for (int i = 0; i < devices.Length; ++i)
        {
            if (!devices[i].isFrontFacing)
            {
                backCamera = new WebCamTexture(devices[i].name, 2976, 2976);
                break;
            }
        }
#endif


        if (backCamera == null)
        {
            Debug.LogError("Unable to find back camera");
            return;
        }

        //background.texture = backCamera;
        screenRenderer.material.mainTexture = backCamera;
        backCamera.Play();
        cameraAvailable = true;
    }

    void Update()
    {
        // debugText.text = "Webcams: " + WebCamTexture.devices.Length.ToString();

        if (cameraAvailable)
        {
            //float ratio = (float)backCamera.width / backCamera.height;
            //fitter.aspectRatio = ratio;

            //float scaleY = backCamera.videoVerticallyMirrored ? -1 : 1;
            //background.rectTransform.localScale = new Vector3(1f, scaleY, 1f);

            //int orient = -backCamera.videoRotationAngle;
            //background.rectTransform.localEulerAngles = new Vector3(0, 0, orient);
        }
    }
}
