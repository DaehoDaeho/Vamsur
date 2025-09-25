using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowSimple : MonoBehaviour
{
    public Transform target;
    public MapBounds mapBounds;
    public float zOffset = -10.0f;
    public Camera cam;

    private void LateUpdate()
    {
        if(target == null)
        {
            return;
        }

        float minX;
        float maxX;
        float minY;
        float maxY;
        mapBounds.GetWorldBounds(out minX, out maxX, out minY, out maxY);

        float halfHeight = cam.orthographicSize;
        float halfWidth = halfHeight * cam.aspect;

        float camMinX = minX + halfWidth;
        float camMaxX = maxX - halfWidth;
        float camMinY = minY + halfHeight;
        float camMaxY = maxY - halfHeight;

        float camX = target.position.x;
        float camY = target.position.y;

        if(camMinX > camMaxX)
        {
            camX = (minX + maxX) * 0.5f;
        }
        else
        {
            camX = Mathf.Clamp(camX, camMinX, camMaxX);
        }

        if (camMinY > camMaxY)
        {
            camY = (minY + maxY) * 0.5f;
        }
        else
        {
            camY = Mathf.Clamp(camY, camMinY, camMaxY);
        }

        transform.position = new Vector3(camX, camY, zOffset);
    }
}
