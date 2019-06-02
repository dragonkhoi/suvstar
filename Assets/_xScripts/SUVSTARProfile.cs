using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SUVSTARProfile : MonoBehaviour
{
    public static SUVSTARProfile Instance;
    public float k1;
    public float k2;

    public float MaxFovOuter;
    public float MaxFovInner;
    public float MaxFovUpper;
    public float MaxFovLower;

    public float LensSeparation;
    public float VerticalLensOffset;
    public float ScreenDistance;

    public float ScreenWidth;
    public float ScreenHeight;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public float distort(float r)
    {
        float r2 = r * r;
        return ((k2 * r2 + k1) * r2 + 1) * r;
    }

    public float distortInv(float radius)
    {
        // Secant method.
        float r0 = 0;
        float r1 = 1;
        float dr0 = radius - distort(r0);
        while (Mathf.Abs(r1 - r0) > 0.0001f)
        {
            float dr1 = radius - distort(r1);
            float r2 = r1 - dr1 * ((r1 - r0) / (dr1 - dr0));
            r0 = r1;
            r1 = r2;
            dr0 = dr1;
        }
        return r1;
    }


/// Calculates the tan-angles from the maximum FOV for the left eye for the
/// current device and screen parameters.
public void GetLeftEyeVisibleTanAngles(float[] result)
    {
        // Tan-angles from the max FOV.
        float fovLeft = Mathf.Tan(-MaxFovOuter * Mathf.Deg2Rad);  //Mathf.Tan(-device.maxFOV.outer * Mathf.Deg2Rad);
        float fovTop = Mathf.Tan(MaxFovUpper * Mathf.Deg2Rad);// Mathf.Tan(device.maxFOV.upper * Mathf.Deg2Rad);
        float fovRight = Mathf.Tan(MaxFovInner * Mathf.Deg2Rad); //Mathf.Tan(device.maxFOV.inner * Mathf.Deg2Rad);
        float fovBottom = Mathf.Tan(-MaxFovLower * Mathf.Deg2Rad); //Mathf.Tan(-device.maxFOV.lower * Mathf.Deg2Rad);
        // Viewport size.
        float halfWidth = ScreenWidth / 4; //creen.width / 4;
        float halfHeight = ScreenHeight / 2; //screen.height / 2;
        // Viewport center, measured from left lens position.
        float centerX = LensSeparation / 2 - halfWidth; // device.lenses.separation / 2 - halfWidth;
        float centerY = -VerticalLensOffset;//-VerticalLensOffset;
        float centerZ = ScreenDistance; // device.lenses.screenDistance;
        // Tan-angles of the viewport edges, as seen through the lens.
        float screenLeft = distort((centerX - halfWidth) / centerZ);
        float screenTop = distort((centerY + halfHeight) / centerZ);
        float screenRight = distort((centerX + halfWidth) / centerZ);
        float screenBottom = distort((centerY - halfHeight) / centerZ);
        // Compare the two sets of tan-angles and take the value closer to zero on each side.
        result[0] = Mathf.Max(fovLeft, screenLeft);
        result[1] = Mathf.Min(fovTop, screenTop);
        result[2] = Mathf.Min(fovRight, screenRight);
        result[3] = Mathf.Max(fovBottom, screenBottom);
    }

    /// Calculates the tan-angles from the maximum FOV for the left eye for the
    /// current device and screen parameters, assuming no lenses.
    public void GetLeftEyeNoLensTanAngles(float[] result)
    {
        // Tan-angles from the max FOV.
        float fovLeft = distortInv(Mathf.Tan(-MaxFovOuter * Mathf.Deg2Rad));
        float fovTop =distortInv(Mathf.Tan(MaxFovUpper * Mathf.Deg2Rad));
        float fovRight = distortInv(Mathf.Tan(MaxFovInner * Mathf.Deg2Rad));
        float fovBottom = distortInv(Mathf.Tan(-MaxFovLower * Mathf.Deg2Rad));
        // Viewport size.
        float halfWidth = ScreenWidth / 4;
        float halfHeight = ScreenHeight / 2;
        // Viewport center, measured from left lens position.
        float centerX = LensSeparation / 2 - halfWidth; // device.lenses.separation / 2 - halfWidth;
        float centerY = -VerticalLensOffset;//-VerticalLensOffset;
        float centerZ = ScreenDistance; // device.lenses.screenDistance;
        // Tan-angles of the viewport edges, as seen through the lens.
        float screenLeft = (centerX - halfWidth) / centerZ;
        float screenTop = (centerY + halfHeight) / centerZ;
        float screenRight = (centerX + halfWidth) / centerZ;
        float screenBottom = (centerY - halfHeight) / centerZ;
        // Compare the two sets of tan-angles and take the value closer to zero on each side.
        result[0] = Mathf.Max(fovLeft, screenLeft);
        result[1] = Mathf.Min(fovTop, screenTop);
        result[2] = Mathf.Min(fovRight, screenRight);
        result[3] = Mathf.Max(fovBottom, screenBottom);
    }

    /// Calculates the screen rectangle visible from the left eye for the
    /// current device and screen parameters.
    public Rect GetLeftEyeVisibleScreenRect(float[] undistortedFrustum)
    {
        float dist = ScreenDistance;
        float eyeX = (ScreenWidth - LensSeparation) / 2;
        float eyeY = VerticalLensOffset + ScreenHeight / 2;
        float left = (undistortedFrustum[0] * dist + eyeX) / ScreenWidth;
        float top = (undistortedFrustum[1] * dist + eyeY) / ScreenHeight;
        float right = (undistortedFrustum[2] * dist + eyeX) / ScreenWidth;
        float bottom = (undistortedFrustum[3] * dist + eyeY) / ScreenHeight;
        return new Rect(left, bottom, right - left, top - bottom);
    }
}
