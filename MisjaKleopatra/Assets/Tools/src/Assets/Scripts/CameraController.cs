using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform follow;
    public float smoothSpeed = 0.15f;
    private Vector3 offset;
    private Vector3 velocity;
    void Start()
    {
        offset = transform.position - follow.position;
    }

    void LateUpdate()
    {
        Vector3 finalPosition = follow.position + offset;       
        transform.position = Vector3.SmoothDamp(transform.position, finalPosition, ref velocity, smoothSpeed);
    }
}
