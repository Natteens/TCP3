using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class StarterAssetsInputs : MonoBehaviour
{
	[Header("Character Input Values")]
	public Vector2 move;
	public Vector2 look;
	public Vector2 rotate;
	public Vector2 zoom;
	public bool jump;
	public bool sprint;
	public bool aim;
	public bool shoot;
	public bool inventory;
	public bool interact;
	public bool reload;

	#region excluir isso dps pfv (SELECAO DE SLOTS HOTBAR)
	public bool slot_1;
	public bool slot_2;
	public bool slot_3;
	public bool slot_4;
	public bool slot_5;
	#endregion

	#region olds

	[Header("Movement Settings")]
    public bool analogMovement;
    #endregion

    public void OnMove(InputValue value)
	{
		MoveInput(value.Get<Vector2>());
	}

	public void OnRotate(InputValue value)
	{
		RotateInput(value.Get<Vector2>());
	}

	public void OnZoom(InputValue value)
	{
		ZoomInput(value.Get<Vector2>());
	}

	public void OnJump(InputValue value)
	{
		JumpInput(value.isPressed);
	}

    public void OnSprint(InputValue value)
	{
		SprintInput(value.isPressed);
	}

	public void OnAim(InputValue value)
	{
		AimInput(value.isPressed);
	}

	public void OnReload(InputValue value)
	{
		ReloadInput(value.isPressed);
	}

    public void OnInventory(InputValue value)
	{
		InventoryInput(value.isPressed);
	}

	public void OnInteract(InputValue value)
	{
		InteractInput(value.isPressed);
	}

	public void OnShoot(InputValue value)
	{
		ShootInput(value.isPressed);
	}

    #region  (SELECAO DE SLOTS HOTBAR)
    public void Slot1(InputValue value)
    {
        Slot1Input(value.isPressed);
    }
    public void Slot2(InputValue value)
    {
        Slot1Input(value.isPressed);
    }
    public void Slot3(InputValue value)
    {
        Slot1Input(value.isPressed);
    }
    public void Slot4(InputValue value)
    {
        Slot1Input(value.isPressed);
    }
    public void Slot5(InputValue value)
    {
        Slot1Input(value.isPressed);
    }

    public void Slot1Input(bool slotValue)
    {
        slot_1 = slotValue;
    }
    public void Slot2Input(bool slotValue)
    {
        slot_2 = slotValue;
    }
    public void Slot3Input(bool slotValue)
    {
        slot_3 = slotValue;
    }
    public void Slot4Input(bool slotValue)
    {
        slot_4 = slotValue;
    }
    public void Slot5Input(bool slotValue)
    {
        slot_5 = slotValue;
    }

    #endregion

    public void MoveInput(Vector2 newMoveDirection)
	{
		move = newMoveDirection;
	}

	public void RotateInput(Vector2 newRotateDirection)
	{
		rotate = newRotateDirection;
	}

	public void ZoomInput(Vector2 newZoomState)
	{
		zoom = newZoomState;
	}


	public void JumpInput(bool newJumpState)
	{
		jump = newJumpState;
	}

	public void InteractInput(bool newInteractState)
	{
		interact = newInteractState;
	}

	public void InventoryInput(bool newInventoryState)
	{
		inventory = newInventoryState;
	}

	public void SprintInput(bool newSprintState)
	{
		sprint = newSprintState;
	}

	public void AimInput(bool newAimState)
	{
		aim = newAimState;
	}

	public void ShootInput(bool newShootState)
	{
		shoot = newShootState;
	}

	private void ReloadInput(bool newReloadState)
	{
		reload = newReloadState;
	}

}