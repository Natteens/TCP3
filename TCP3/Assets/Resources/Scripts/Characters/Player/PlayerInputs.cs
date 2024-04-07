using UnityEngine;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;
using Unity.Netcode;
using Mono.CSharp.yyParser;

public class PlayerInputs : MonoBehaviour
{
    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction aimAction;

    public InputAction AimAction
    {
        get { return aimAction; }
        set { aimAction = value; }
    }

    public InputAction MoveAction
    {
        get { return moveAction; }
        set { moveAction = value; }
    }
    public InputAction JumpAction
    {
        get { return jumpAction; }
        set { jumpAction = value; }
    }

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();

        moveAction = playerInput.actions["Move"];
        jumpAction = playerInput.actions["Jump"];
        aimAction = playerInput.actions["Aim"];
    }

}
