using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapPlayer : MonoBehaviour
{
    public Transform target;
    public Camera minimapCamera;

    public RectTransform minimapRect;
    public RectTransform iconPlayer;

    // Update is called once per frame
    void Update()
    {
        float halfH = minimapCamera.orthographicSize;
        float halfW = halfH * minimapCamera.aspect;
        float viewWidth = halfW * 2.0f;
        float viewHeight = halfH * 2.0f;

        Vector3 camPos = minimapCamera.transform.position;
        Vector3 p = target.position;
        float dx = p.x - camPos.x;
        float dy = p.y - camPos.y;

        float tx = dx / viewWidth;  // -0.5 ~ 0.5
        float ty = dy / viewHeight;

        float panelW = minimapRect.rect.width;
        float panelH = minimapRect.rect.height;
        float px = tx * panelW;
        float py = ty * panelH;

        float halfPanelW = panelW * 0.5f;
        float halfPanelH = panelH * 0.5f;

        if(px < -halfPanelW)
        {
            px = -halfPanelW;
        }

        if (px > halfPanelW)
        {
            px = halfPanelW;
        }

        if (py < -halfPanelH)
        {
            py = -halfPanelH;
        }

        if (py > halfPanelH)
        {
            py = halfPanelH;
        }

        iconPlayer.anchoredPosition = new Vector2(px, py);
    }
}
