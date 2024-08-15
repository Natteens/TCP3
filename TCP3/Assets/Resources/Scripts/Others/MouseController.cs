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

    public static void CursorVisibility(bool visibility)
    {
        Cursor.visible = visibility;
    }

    public static (bool success, Vector3 position) GetMousePosition(Camera camera, LayerMask layer)
    {
        var ray = camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, layer))
        {
            // The Raycast hit something, return with the position.
            return (success: true, position: hitInfo.point);
        }
        else
        {
            // The Raycast did not hit anything.
            return (success: false, position: Vector3.zero);
        }
    }
}
