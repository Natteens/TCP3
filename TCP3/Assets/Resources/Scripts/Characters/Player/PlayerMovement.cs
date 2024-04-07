using UnityEngine;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;
using Unity.Netcode;

[RequireComponent(typeof(CharacterController), typeof(PlayerInput))]
public class PlayerMovement : NetworkBehaviour
{
    [BoxGroup("Movement Values")]
    [SerializeField]
    private float playerSpeed = 2.0f;
    [BoxGroup("Movement Values")]
    [SerializeField]
    private float jumpHeight = 1.0f;
    [BoxGroup("Movement Values")]
    [SerializeField]
    private float gravityValue = -9.81f;
    [BoxGroup("Movement Values")]
    [SerializeField]
    private float rotationSpeed = 5f;

    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private Transform cameraTransform;

    private PlayerInputs myInputs;

    

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        myInputs = GetComponent<PlayerInputs>();
        cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        /* Descomentar quando passar pra mult
        if (IsOwner)
        {
            Movement();
        }
        */

        Movement();
        
    }

    private void Movement()
    {
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        Vector2 input = myInputs.MoveAction.ReadValue<Vector2>();
        Vector3 move = new Vector3(input.x, 0, input.y);
        move = move.x * cameraTransform.right.normalized + move.z * cameraTransform.forward.normalized;
        move.y = 0f;
        controller.Move(move * Time.deltaTime * playerSpeed);

        if (myInputs.JumpAction.triggered && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        Quaternion targetRotation = Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}