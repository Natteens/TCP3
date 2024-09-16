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

    public static (bool success, Vector3 position) GetMousePosition(Camera camera, LayerMask layer, float maxDistance = Mathf.Infinity)
    {
        // Cria um ray a partir da posição do mouse na tela
        var ray = camera.ScreenPointToRay(Input.mousePosition);

        // Realiza o Raycast e verifica se houve colisão
        if (Physics.Raycast(ray, out var hitInfo, maxDistance, layer))
        {
            // Se o Raycast acertou um objeto, retorna a posição do hit
            return (success: true, position: hitInfo.point);
        }
        else
        {
            // Se não acertou nenhum objeto, retorna a posição zero
            return (success: false, position: Vector3.zero);
        }
    }
}
