using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public static class MouseController
{
    public static void DisableMouse()
    { 
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Debug.Log("#Mouse Desativado#");
    }

    public static void ActiveMouse()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Debug.Log("#Mouse Ativado#");
    }
}
