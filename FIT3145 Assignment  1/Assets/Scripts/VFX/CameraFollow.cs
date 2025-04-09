using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform playerPosition;
    private Vector3 cameraWorldPosition;
    [SerializeField] private float maxDistanceToPlayer = 20f;
    [SerializeField] private float maxCameraDistance = 10f;

    private void Awake()
    {
        playerPosition = GameObject.FindWithTag("Player").transform;
    }

    private void Update()
    {
        if (playerPosition == null) return;
        cameraWorldPosition = GetMouseWorldPosition();
        CalculateCameraPosition();
    }
    
    private void CalculateCameraPosition()
    {
        Vector3 middlePoint = (playerPosition.position + cameraWorldPosition) / 2f;

        // Calculate the distance between the middle point and the player
        float distanceToMiddle = Vector3.Distance(playerPosition.position, middlePoint);

        // If the distance is greater than maxDistanceToPlayer, clamp it
        if (distanceToMiddle > maxDistanceToPlayer)
        {
            middlePoint = playerPosition.position + (middlePoint - playerPosition.position).normalized * maxDistanceToPlayer;
        }

        // Calculate the distance between the camera and the middle point
        float cameraDistance = Vector3.Distance(transform.position, middlePoint);

        // If the camera distance is greater than maxCameraDistance, move the camera closer
        if (cameraDistance > maxCameraDistance)
        {
            transform.position = playerPosition.position + (transform.position - playerPosition.position).normalized * maxCameraDistance;
        }
        else
        {
            // Move the object (camera follow target) to the middle point
            transform.position = middlePoint;
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = 0f;
        return mouseWorldPosition;
    }
}