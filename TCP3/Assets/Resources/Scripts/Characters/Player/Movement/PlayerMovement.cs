using UnityEngine;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;
using Unity.Netcode;

[RequireComponent(typeof(CharacterController), typeof(PlayerInput))]
public class PlayerMovement : NetworkBehaviour
{
    //[SerializeField]
    [BoxGroup("Movement Values")]
    private float playerSpeed = 2.0f;

    [SerializeField]
    [BoxGroup("Movement Values")]
    private float jumpHeight = 1.0f;

    [SerializeField]
    [BoxGroup("Movement Values")]
    private float gravityValue = -9.81f;

    [SerializeField]
    [BoxGroup("Movement Values")]
    private float rotationSpeed = 5f;

    [SerializeField] private CharacterController controller;
    [SerializeField] private PlayerInputs myInputs;
    [SerializeField] private PlayerManager myManager;

    private Vector3 playerVelocity;
    private bool groundedPlayer;
    [SerializeField] private Transform cameraTransform;

    private void Awake()
    {
        cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        if (IsOwner)
        {
            SetupCamera();
            Movement();
        }      
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

            myManager.EstaminaAtual -= 30;
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        Quaternion targetRotation = Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    private void SetupCamera()
    {
        try
        {
            if (myInputs != null) { SwitchVCam.Instance.SetInputs(myInputs); }
        }
        catch (System.Exception e)
        {
            Debug.Log(e);
            throw;
        }
    }

}
