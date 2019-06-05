using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTransform : MonoBehaviour
{
    public Transform toFollow;
    public bool followX;
    public bool followY;
    public bool followZ;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 newPosition = new Vector3(); 
        newPosition.x = followX ? toFollow.position.x : transform.position.x;
        newPosition.y = followY ? toFollow.position.y : transform.position.y;
        newPosition.z = followZ ? toFollow.position.z : transform.position.z;
        transform.position = newPosition;
    }
}
