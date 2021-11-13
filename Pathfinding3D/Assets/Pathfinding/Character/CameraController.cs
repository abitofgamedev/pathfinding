using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform trackPoint;
    public float FollowSpeed;
    public Vector3 offset;
    private void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, trackPoint.position- (trackPoint.forward+offset), FollowSpeed*Time.deltaTime);
        transform.LookAt(trackPoint);
    }
}
