using UnityEngine;

public class GPTMOVIMENTO : MonoBehaviour
{
    public float velocidadeMovimento = 5f; // Velocidade de movimento do objeto
    public float sensibilidadeMouse = 2f; // Sensibilidade do mouse para rotação

    private float rotationX = 0f;

    //void Start()
    //{
    //    // Esconder e travar o cursor do mouse
    //    Cursor.lockState = CursorLockMode.Locked;
    //}

    void Update()
    {
        // Movimento do jogador
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 moveDirection = transform.right * moveHorizontal + transform.forward * moveVertical;
        transform.Translate(moveDirection * velocidadeMovimento * Time.deltaTime, Space.World);

        // Rotação do jogador com base no mouse
        float mouseX = Input.GetAxis("Mouse X") * sensibilidadeMouse;
        rotationX -= Input.GetAxis("Mouse Y") * sensibilidadeMouse;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);

        transform.Rotate(Vector3.up * mouseX);
        Camera.main.transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
    }
}
