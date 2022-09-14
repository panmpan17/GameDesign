using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;


[RequireComponent(typeof(CinemachineVirtualCamera))]
public class CameraRole : MonoBehaviour
{
    [SerializeField]
    private string cameraName;
    public string CameraName => cameraName;

    private CinemachineVirtualCamera _camera;

    void Awake()
    {
        CameraSwitcher.RegisterCamera(this);
        _camera = GetComponent<CinemachineVirtualCamera>();
        _camera.enabled = false;
    }

    public void Enable()
    {
        _camera.enabled = true;
    }
    public void Disable()
    {
        _camera.enabled = false;
    }
}
