using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : IInputHandler
{
    private PlayerInput playerInput;
    private Vector2 moveDirection;
    private Vector2 lookDirection;
    private bool jumpPressed;
    private bool attackPressed;
    private bool crouchPressed;
    private bool interactPressed;

    public PlayerInputHandler(PlayerInput input)
    {
        playerInput = input;
        SubscribeToInputEvents();
    }

    private void SubscribeToInputEvents()
    {
        var playerActions = playerInput.actions.FindActionMap("Player");
        playerActions.FindAction("Move").performed += OnMove;
        playerActions.FindAction("Look").performed += OnLook;
        playerActions.FindAction("Jump").performed += OnJump;
        playerActions.FindAction("Attack").performed += OnAttack;
        playerActions.FindAction("Crouch").performed += OnCrouch;
        playerActions.FindAction("Interact").performed += OnInteract;
    }

    private void OnMove(InputAction.CallbackContext context) => moveDirection = context.ReadValue<Vector2>();
    private void OnLook(InputAction.CallbackContext context) => lookDirection = context.ReadValue<Vector2>();
    private void OnJump(InputAction.CallbackContext context) => jumpPressed = true;
    private void OnAttack(InputAction.CallbackContext context) => attackPressed = true;
    private void OnCrouch(InputAction.CallbackContext context) => crouchPressed = true;
    private void OnInteract(InputAction.CallbackContext context) => interactPressed = true;

    public Vector2 GetMoveDirection() => moveDirection;
    public Vector2 GetLookDirection() => lookDirection;
    public bool IsJumpPressed() => jumpPressed;
    public bool IsAttackPressed() => attackPressed;
    public bool IsCrouchPressed() => crouchPressed;
    public bool IsInteractPressed() => interactPressed;
}