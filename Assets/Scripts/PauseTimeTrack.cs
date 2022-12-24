using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PauseTimeTrack
{
    private static List<MonoBehaviour> _triggerPauseComponents;

    public static void Pause(MonoBehaviour monoBehaviour)
    {
        Time.timeScale = 0;
        GameManager.ins.Player.Input.Disable();

        _triggerPauseComponents ??= new List<MonoBehaviour>();
        _triggerPauseComponents.Add(monoBehaviour);
    }

    public static void Unpause(MonoBehaviour monoBehaviour)
    {
        _triggerPauseComponents.Remove(monoBehaviour);

        for (int i = 0; i < _triggerPauseComponents.Count; i++)
        {
            if (!_triggerPauseComponents[i])
            {
                _triggerPauseComponents.RemoveAt(i);
                i--;
            }
        }

        if (_triggerPauseComponents.Count <= 0)
        {
            Time.timeScale = 1;
            GameManager.ins.Player.Input.Enable();
        }
    }
}
