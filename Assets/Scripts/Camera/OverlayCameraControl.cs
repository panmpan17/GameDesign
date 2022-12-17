using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Camera))]
public class OverlayCameraControl : MonoBehaviour
{
    [SerializeField]
    private Camera targetCamera;
    private Camera _selfCamera;

    void Awake()
    {
        _selfCamera = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        _selfCamera.nearClipPlane = targetCamera.nearClipPlane;
        _selfCamera.farClipPlane = targetCamera.farClipPlane;
        _selfCamera.fieldOfView = targetCamera.fieldOfView;
    }
}
