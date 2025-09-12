using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBounds : MonoBehaviour
{
    public Renderer mapRenderer;

    public void GetWorldBounds(out float outMinX, out float outMaxX, out float outMinY, out float outMaxY)
    {
        Bounds bound = mapRenderer.bounds;
        outMinX = bound.min.x;
        outMaxX = bound.max.x;
        outMinY = bound.min.y;
        outMaxY = bound.max.y;
    }
}
