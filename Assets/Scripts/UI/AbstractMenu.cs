using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;


public abstract class AbstractMenu : MonoBehaviour
{
    public static event System.Action<AbstractMenu> OnFirstMenuOpen;
    public static event System.Action<AbstractMenu> OnLastMenuClose;

    private static Stack<AbstractMenu> s_openedMenus = new Stack<AbstractMenu>(6);

    protected void OpenMenu()
    {
        s_openedMenus.Push(this);
        if (s_openedMenus.Count == 1)
        {
            OnFirstMenuOpen?.Invoke(this);
        }
    }

    protected void CloseMenu()
    {
        s_openedMenus.Pop();
        if (s_openedMenus.Count == 0)
        {
            OnLastMenuClose?.Invoke(this);
        }
    }
}
