using Fusion;
using UnityEngine;
using Cinemachine;

public class GPTMOVIMENTO : NetworkBehaviour
{
    public Camera cam;
    public float velocidadeMovimento = 5f; // Velocidade de movimento do objeto
    public float sensibilidadeMouse = 2f; // Sensibilidade do mouse para rotação

    private float rotationX = 0f;

    //void Start()
    //{
    //    // Esconder e travar o cursor do mouse
    //    Cursor.lockState = CursorLockMode.Locked;
    //}
    //public override void Spawned()
    //{
    //    if (HasStateAuthority)
    //    {
    //        cam.GetComponentInChildren<CinemachineVirtualCamera>().Follow = transform;
    //    }
    //}
    public override void FixedUpdateNetwork()
    {
        if (HasStateAuthority == false)
        {
            return;
        }

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
        cam.transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
    }
}
