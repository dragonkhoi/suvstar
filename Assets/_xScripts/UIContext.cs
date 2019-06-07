using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIContext : MonoBehaviour
{
    public GameObject Left;
    public GameObject Right;

    private Transform currentBg;
    public Transform rightBg;
    public Transform leftBg;

    private Transform belowFold;
    public Transform leftBelowFold;
    public Transform rightBelowFold;

    private Vector3 ogScale;
    public Vector3 finalScale;

    // Start is called before the first frame update
    void Start()
    {
        currentBg = Left.activeInHierarchy ? leftBg : rightBg;
        ogScale = currentBg.localScale;
        belowFold = Left.activeInHierarchy ? leftBelowFold : rightBelowFold;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Expand()
    {
        StopCoroutine(CollapseUI());
        StartCoroutine(ExpandUI());
    }

    IEnumerator ExpandUI()
    {
        Vector3 currentScale = currentBg.localScale;
        int i = 0;
        while (i <= 100f)
        {
            currentBg.localScale = Vector3.Lerp(currentScale, finalScale, i / 100f);
            for (int k = 0; k < belowFold.childCount; k++)
            {
                belowFold.GetChild(k).GetComponent<TextMesh>().color = Vector4.Lerp(new Vector4(1f,1f,1f,0f), new Vector4(1f, 1f, 1f, 1f), i / 100f);
            }
            yield return new WaitForSeconds(0.0005f);
            i++;
        } 
    }

    IEnumerator CollapseUI()
    {
        Vector3 currentScale = currentBg.localScale;

        int i = 0;
        while (i <= 100f)
        {
            currentBg.localScale = Vector3.Lerp(currentScale, ogScale, i / 100f);
            for (int k = 0; k < belowFold.childCount; k++)
            {
                belowFold.GetChild(k).GetComponent<TextMesh>().color = Vector4.Lerp(new Vector4(1f, 1f, 1f, 1f), new Vector4(1f, 1f, 1f, 0f), i / 100f);
            }
            yield return new WaitForSeconds(0.0005f);
            i++;
        }
    }

    public void Collapse()
    {
        StopCoroutine(ExpandUI());
        StartCoroutine(CollapseUI());
    }
}
