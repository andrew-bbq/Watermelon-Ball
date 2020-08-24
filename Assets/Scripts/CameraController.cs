using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float zoomSpeed = 0.2f;
    private Func<Vector3> getCameraPosition;
    public void Setup(Func<Vector3> getCameraPosition) {
        this.getCameraPosition = getCameraPosition;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 cameraPosition = getCameraPosition();
        cameraPosition.z = transform.position.z;
        transform.position = cameraPosition;
        // handle zoom out
        float currentSize =GetComponent<Camera>().orthographicSize;
        currentSize -= Input.mouseScrollDelta.y * zoomSpeed;
        currentSize = Mathf.Clamp(currentSize, 2, 12);
        GetComponent<Camera>().orthographicSize = currentSize;
    }
}
