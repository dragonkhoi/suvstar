using UnityEngine;

/* Adjusts EyeCamera Z position based on slider UI. */
public class SetTransform : MonoBehaviour
{
    float ogZ;
    public UnityEngine.UI.Text eyeCamZText;

    void Start()
    {
        ogZ = transform.position.z;
    }

    public void SetZ(float z)
    {
        float newZ = (float)System.Math.Round(ogZ + z, 1);
        transform.position = new Vector3(transform.position.x, transform.position.y, newZ );
        eyeCamZText.text = "Eye Camera Z: " + newZ.ToString();
    }
}
