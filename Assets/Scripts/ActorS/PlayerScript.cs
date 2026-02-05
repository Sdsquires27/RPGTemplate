using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerScript : ActorScript, IPlayerInput
{
    private IInputHandler inputHandler;

    protected override void Start()
    {
        base.Start();
        inputHandler = GameServices.GetInputHandler();
        
        var playerInput = GameServices.GetPlayerInput();
        playerInput.actions.Disable();
        playerInput.actions.FindActionMap("Player").Enable();
    }

    protected override void HandleMovement()
    {
        Vector2 moveDirection = inputHandler.GetMoveDirection();
        rb.linearVelocity = moveDirection * moveSpeed;
    }

    protected override void HandleActions()
    {
        if (inputHandler.IsInteractPressed())
        {
            OnInteract();
        }
        
        if (inputHandler.IsAttackPressed())
        {
            OnAttack();
        }
        
        if (inputHandler.IsJumpPressed())
        {
            OnJump();
        }
    }

    public void OnAttack()
    {
        Debug.Log("Player attacked!");
    }

    public void OnCrouch()
    {
        Debug.Log("Player crouched!");
    }

    public void OnInteract()
    {
        Debug.Log("Player interacted!");
    }

    public void OnJump()
    {
        Debug.Log("Player jumped!");
    }

    public void OnLook(Vector2 direction)
    {
        Debug.Log($"Player looking: {direction}");
    }

    public void OnMove(Vector2 direction)
    {
        // Handled in HandleMovement()
    }
}