using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    [SerializeField]
    private PlayerInput playerInput;

    public bool CursorFocued { get; protected set; }

    void Awake()
    {
        playerInput.OnAimDown += OnAimDown;

        playerInput.OnOutFocus += OnOutFocus;
    }



    void OnAimDown()
    {
        if (!CursorFocued)
        {
            CursorFocued = true;
            Cursor.lockState = CursorLockMode.Locked;
            return;
        }
    }

    void OnOutFocus()
    {
        CursorFocued = false;
        Cursor.lockState = CursorLockMode.None;
    }
}
