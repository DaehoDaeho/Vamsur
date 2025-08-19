using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardToCamera : MonoBehaviour
{
    public Camera targetCamera;

    private void LateUpdate()
    {
        if(targetCamera == null)
        {
            targetCamera = Camera.main;
        }

        if(targetCamera == null)
        {
            return;
        }

        transform.forward = targetCamera.transform.forward;
    }
}
