using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapCameraFollow : MonoBehaviour
{
    public MapBounds mapBounds;
    public Transform target;
    public bool keepNorthUp = true;
    public float zOffset = -10.0f;

    public Camera cam;

    public float zoomIn = 4.0f;
    public float zoomOut = 30.0f;
    public float zoomStep = 2.0f;

    private void Update()
    {
        // ¹Ì´Ï¸Ê ÁÜ ÀÎ/ÁÜ ¾Æ¿ô ±â´É.
        float wheel = Input.mouseScrollDelta.y;
        if(wheel != 0.0f)
        {
            float size = cam.orthographicSize;
            size = size - (wheel * zoomStep);
            size = Mathf.Clamp(size, zoomIn, zoomOut);
            cam.orthographicSize = size;
        }
    }

    private void LateUpdate()
    {
        float minX;
        float maxX;
        float minY;
        float maxY;

        mapBounds.GetWorldBounds(out minX, out maxX, out minY, out maxY);

        float halfH = cam.orthographicSize;
        float halfW = halfH * cam.aspect;

        float camMinX = minX + halfW;
        float camMaxX = maxX - halfW;
        float camMinY = minY + halfH;
        float camMaxY = maxY - halfH;

        float cx = target.position.x;
        float cy = target.position.y;

        if(camMinX > camMaxX)
        {
            cx = (minX + maxX) * 0.5f;
        }
        else
        {
            cx = Mathf.Clamp(cx, camMinX, camMaxX);
        }

        if (camMinY > camMaxY)
        {
            cy = (minY + maxY) * 0.5f;
        }
        else
        {
            cy = Mathf.Clamp(cy, camMinY, camMaxY);
        }

        transform.position = new Vector3(cx, cy, zOffset);
    }
}
