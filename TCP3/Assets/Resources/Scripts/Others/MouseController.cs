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
        // Cria um ray a partir da posi��o do mouse na tela
        var ray = camera.ScreenPointToRay(Input.mousePosition);

        // Realiza o Raycast e verifica se houve colis�o
        if (Physics.Raycast(ray, out var hitInfo, maxDistance, layer))
        {
            // Se o Raycast acertou um objeto, retorna a posi��o do hit
            return (success: true, position: hitInfo.point);
        }
        else
        {
            // Se n�o acertou nenhum objeto, retorna a posi��o zero
            return (success: false, position: Vector3.zero);
        }
    }
}
