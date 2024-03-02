using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [SerializeField] Vector3 offset;
    [SerializeField] float smoothTime;
    Vector3 velocity = Vector3.zero;
    [SerializeField] Transform player;

    private void FixedUpdate()
    {
        Vector3 targetPos = player.position + offset; // Camera follow player with offset position
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smoothTime);
    }

    
}
