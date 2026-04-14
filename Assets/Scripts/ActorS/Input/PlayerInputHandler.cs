using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : IInputHandler
{
    private PlayerInput playerInput;
    private bool inputEnabled = true;


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

    public void DisableInput() => inputEnabled = false;
    public void EnableInput() => inputEnabled = true;
    public Vector2 GetMoveDirection() => inputEnabled ? moveDirection : Vector2.zero;
    public Vector2 GetLookDirection() => inputEnabled ? lookDirection : Vector2.zero;
    public bool IsJumpPressed() => inputEnabled && jumpPressed;
    public bool IsAttackPressed() => inputEnabled && attackPressed;
    public bool IsCrouchPressed() => inputEnabled && crouchPressed;
    public bool IsInteractPressed() => inputEnabled && interactPressed;
}