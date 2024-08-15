using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugMovement : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float mouseSensitivity = 100f;

    private float yaw = 0f;   // Rota��o no eixo Y (horizontal)
    private float pitch = 0f; // Inclina��o no eixo X (vertical)

    void Start()
    {
        // Trava o cursor no centro da tela e o deixa invis�vel
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Movimento de transla��o
        HandleMovement();

        // Rota��o da c�mera
        HandleCameraRotation();
    }

    void HandleMovement()
    {
        // Movimenta��o nas dire��es baseadas no input do teclado
        float moveX = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        float moveZ = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;

        // Move a c�mera para frente/tr�s e esquerda/direita
        transform.Translate(moveX, 0f, moveZ);
    }

    void HandleCameraRotation()
    {
        // Captura o movimento do mouse
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Ajusta a rota��o da c�mera
        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -90f, 90f); // Limita a rota��o vertical

        // Aplica a rota��o ao transform da c�mera
        transform.eulerAngles = new Vector3(pitch, yaw, 0f);
    }
}
