using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugMovement : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float mouseSensitivity = 100f;

    private float yaw = 0f;   // Rotação no eixo Y (horizontal)
    private float pitch = 0f; // Inclinação no eixo X (vertical)

    void Start()
    {
        // Trava o cursor no centro da tela e o deixa invisível
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Movimento de translação
        HandleMovement();

        // Rotação da câmera
        HandleCameraRotation();
    }

    void HandleMovement()
    {
        // Movimentação nas direções baseadas no input do teclado
        float moveX = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        float moveZ = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;

        // Move a câmera para frente/trás e esquerda/direita
        transform.Translate(moveX, 0f, moveZ);
    }

    void HandleCameraRotation()
    {
        // Captura o movimento do mouse
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Ajusta a rotação da câmera
        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -90f, 90f); // Limita a rotação vertical

        // Aplica a rotação ao transform da câmera
        transform.eulerAngles = new Vector3(pitch, yaw, 0f);
    }
}
