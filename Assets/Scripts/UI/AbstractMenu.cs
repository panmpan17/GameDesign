using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;


public abstract class AbstractMenu : MonoBehaviour
{
    public static event System.Action<AbstractMenu> OnFirstMenuOpen;
    public static event System.Action<AbstractMenu> OnLastMenuClose;

    private static Stack<AbstractMenu> s_openedMenus = new Stack<AbstractMenu>(6);
    private static List<AbstractMenu> s_existMenus = new List<AbstractMenu>();

    public static void S_OpenMenu(string menuName)
    {
        for (int i = 0; i < s_existMenus.Count; i++)
        {
            if (s_existMenus[i].menuName == menuName)
            {
                s_existMenus[i].OpenMenu();
                return;
            }
        }
    }


    [SerializeField]
    private string menuName;


    protected void RegisterMenu()
    {
        s_existMenus.Add(this);
    }

    protected virtual void OpenMenu()
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
        else
        {
            s_openedMenus.Peek().BackToThisMenu();
        }
    }

    protected virtual void BackToThisMenu()
    {}
}
