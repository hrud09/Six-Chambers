using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class DynamicCamera : MonoBehaviour
{
    private float screenRatio = 0f;
    private const float baseResolutionWidth = 1080f;
    private const float baseResolutionHeight = 1920f;
    private const float baseOrthographicSize = 5f;

    void Start()
    {
        Camera cam = GetComponent<Camera>();
        Vector3 pos = cam.transform.position;

        screenRatio = (float)Screen.height / Screen.width;

        AdjustCameraSize(cam);
    }

    void AdjustCameraSize(Camera cam)
    {
        float targetRatio = baseResolutionHeight / baseResolutionWidth;
        float differenceInSize = screenRatio / targetRatio;
        cam.orthographicSize = baseOrthographicSize * differenceInSize;
    }

}
