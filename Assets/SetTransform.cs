using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetTransform : MonoBehaviour
{
    float ogZ;
    public UnityEngine.UI.Text eyeCamZText;
    // Start is called before the first frame update
    void Start()
    {
        ogZ = transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetZ(float z)
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, ogZ + z);
        eyeCamZText.text = "Eye Camera Z: " + (ogZ + z).ToString();
    }
}
