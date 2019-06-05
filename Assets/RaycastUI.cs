using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastUI : MonoBehaviour
{
    Transform hitTransform = null;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 2f))
        {
            if (hitTransform != hit.transform)
            {
                hit.transform.GetComponent<UIContext>().Expand();
                hitTransform = hit.transform;
            }
        }
        else
        {
            if (hitTransform != null)
            {
                hitTransform.GetComponent<UIContext>().Collapse();
            }
            hitTransform = null;
        }
    }
}
