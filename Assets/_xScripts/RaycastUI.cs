using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastUI : MonoBehaviour
{
    Transform hitTransform = null;
    private float dampTime = 5f;
    private float currentTimer = 0f;
    public Transform pivotPointUI;
    public Transform cameraOffset;
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
                if (hit.transform.GetComponent<UIContext>() != null)
                {
                    hit.transform.GetComponent<UIContext>().Expand();
                    hitTransform = hit.transform;
                    currentTimer = 0;
                }
                
            }
            else
            {
                if (hit.transform.GetComponent<UIContext>() != null)
                {
                    currentTimer = 0;
                }
            }
        }
        else
        {
            if (hitTransform != null)
            {
                hitTransform.GetComponent<UIContext>().Collapse();
            }
            else
            {
                currentTimer += Time.deltaTime;
                if (currentTimer > dampTime)
                {
                    // This wasn't working due to strange axes misalignments on the phones
                    //StopCoroutine(RotatePivotPoint());
                    //StartCoroutine(RotatePivotPoint());
                    currentTimer = 0;
                }
            }
            hitTransform = null;
        }
    }

    IEnumerator RotatePivotPoint()
    {
        int i = 0;
        while ((pivotPointUI.forward - cameraOffset.forward).magnitude > 0.005f)
        {
            yield return new WaitForSeconds(0.005f);
            pivotPointUI.forward = Vector3.RotateTowards(pivotPointUI.forward, cameraOffset.forward, 0.05f, 0.2f);
            i++;
        }
        currentTimer = 0f;
    }
}
