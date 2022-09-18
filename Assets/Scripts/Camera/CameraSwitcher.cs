using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraSwitcher : MonoBehaviour
{
    private static List<CameraRole> s_cameras = new List<CameraRole>();
    private static List<string> s_cameraNames = new List<string>();

    public static int RegisterCamera(CameraRole cameraRole)
    {
        Debug.Log(cameraRole.CameraName);
        for (int i = 0; i < s_cameraNames.Count; i++)
        {
            if (s_cameraNames[i] == cameraRole.CameraName)
            {
                s_cameras[i] = cameraRole;
                return i;
            }
        }

        s_cameras.Add(cameraRole);
        s_cameraNames.Add(cameraRole.CameraName);
        return s_cameras.Count - 1;
    }

    public static int GetCameraIndex(string name)
    {
        for (int i = 0; i < s_cameraNames.Count; i++)
        {
            if (s_cameraNames[i] == name)
                return i;
        }

        s_cameras.Add(null);
        s_cameraNames.Add(name);
        return s_cameras.Count - 1;
    }

    public static CameraSwitcher ins;

    [SerializeField]
    private CameraRole startCamera;
    [SerializeField]
    private string startCameraName;
    private int _currentIndex;


    void Awake()
    {
        ins = this;
    }

    void Start()
    {
        if (startCamera)
        {
            _currentIndex = GetCameraIndex(startCamera.name);
            startCamera.Enable();
        }
        else
        {
            _currentIndex = GetCameraIndex(startCameraName);
            s_cameras[_currentIndex].Enable();
        }
    }

    public void SwitchTo(string cameraName)
    {
        s_cameras[_currentIndex]?.Disable();

        _currentIndex = GetCameraIndex(cameraName);
        s_cameras[_currentIndex]?.Enable();
    }

    public void SwitchTo(int cameraIndex)
    {
        s_cameras[_currentIndex]?.Disable();

        _currentIndex = cameraIndex;
        s_cameras[_currentIndex]?.Enable();
    }
}
