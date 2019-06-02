using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetTransform : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void ResetTransformToZero()
    {
        transform.rotation = Quaternion.Euler(new Vector3(0f, 0f,  0f));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
